namespace AtlasOps.Features.Messaging.MessageBrokerRecovery;

using AtlasOps.Features;

public sealed class MessageBrokerRecoveryService(
    IAtlasOpsCapabilityRepository<MessageBrokerRecoveryItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageBrokerRecoveryValidator validator = new();
    private readonly MessageBrokerRecoveryPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageBrokerRecoveryChanged>> ExecuteAsync(
        UpdateMessageBrokerRecoveryCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageBrokerRecoveryChanged>.Invalid(issues);
        }

        MessageBrokerRecoveryItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageBrokerRecoveryItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageBrokerRecoveryChanged>.Invalid(
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

        MessageBrokerRecoveryChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageBrokerRecoveryChanged>.Success(changed);
    }
}