namespace AtlasOps.Features.Messaging.MessageSubscriptionRecovery;

using AtlasOps.Features;

public sealed class MessageSubscriptionRecoveryService(
    IAtlasOpsCapabilityRepository<MessageSubscriptionRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageSubscriptionRecoveryValidator validator = new();
    private readonly MessageSubscriptionRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageSubscriptionRecoveryChanged>> ExecuteAsync(
        UpdateMessageSubscriptionRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageSubscriptionRecoveryChanged>.Invalid(issues);
        }

        MessageSubscriptionRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageSubscriptionRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageSubscriptionRecoveryChanged>.Invalid(
            [
                new("State", $"Cannot transition from '{previousState}' to '{command.TargetState}'."),
            ]);
        }

        entity.Name = command.Name.Trim();
        entity.Owner = command.Owner.Trim();
        entity.State = command.TargetState;
        entity.Priority = command.Priority;
        entity.IsEnabled = command.IsEnabled;
        DateTimeOffset now = timeProvider.GetUtcNow();
        entity.MarkUpdated(now);
        await repository.SaveAsync(entity, cancellationToken);

        MessageSubscriptionRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageSubscriptionRecoveryChanged>.Success(changed);
    }
}