namespace AtlasOps.Features.Messaging.MessageConsumerRecovery;

using AtlasOps.Features;

public sealed class MessageConsumerRecoveryService(
    IAtlasOpsCapabilityRepository<MessageConsumerRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageConsumerRecoveryValidator validator = new();
    private readonly MessageConsumerRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageConsumerRecoveryChanged>> ExecuteAsync(
        UpdateMessageConsumerRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageConsumerRecoveryChanged>.Invalid(issues);
        }

        MessageConsumerRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageConsumerRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageConsumerRecoveryChanged>.Invalid(
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

        MessageConsumerRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageConsumerRecoveryChanged>.Success(changed);
    }
}