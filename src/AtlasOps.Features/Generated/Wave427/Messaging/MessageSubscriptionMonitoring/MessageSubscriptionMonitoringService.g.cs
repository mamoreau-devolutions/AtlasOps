namespace AtlasOps.Features.Messaging.MessageSubscriptionMonitoring;

using AtlasOps.Features;

public sealed class MessageSubscriptionMonitoringService(
    IAtlasOpsCapabilityRepository<MessageSubscriptionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageSubscriptionMonitoringValidator validator = new();
    private readonly MessageSubscriptionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageSubscriptionMonitoringChanged>> ExecuteAsync(
        UpdateMessageSubscriptionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageSubscriptionMonitoringChanged>.Invalid(issues);
        }

        MessageSubscriptionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageSubscriptionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageSubscriptionMonitoringChanged>.Invalid(
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

        MessageSubscriptionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageSubscriptionMonitoringChanged>.Success(changed);
    }
}