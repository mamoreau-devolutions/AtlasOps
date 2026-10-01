namespace AtlasOps.Features.Messaging.MessageQueueRecovery;

using AtlasOps.Features;

public sealed class MessageQueueRecoveryService(
    IAtlasOpsCapabilityRepository<MessageQueueRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageQueueRecoveryValidator validator = new();
    private readonly MessageQueueRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageQueueRecoveryChanged>> ExecuteAsync(
        UpdateMessageQueueRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageQueueRecoveryChanged>.Invalid(issues);
        }

        MessageQueueRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageQueueRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageQueueRecoveryChanged>.Invalid(
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

        MessageQueueRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageQueueRecoveryChanged>.Success(changed);
    }
}