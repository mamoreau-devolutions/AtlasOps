namespace AtlasOps.Features.Messaging.MessageBrokerMonitoring;

using AtlasOps.Features;

public sealed class MessageBrokerMonitoringService(
    IAtlasOpsCapabilityRepository<MessageBrokerMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageBrokerMonitoringValidator validator = new();
    private readonly MessageBrokerMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageBrokerMonitoringChanged>> ExecuteAsync(
        UpdateMessageBrokerMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageBrokerMonitoringChanged>.Invalid(issues);
        }

        MessageBrokerMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageBrokerMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageBrokerMonitoringChanged>.Invalid(
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

        MessageBrokerMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageBrokerMonitoringChanged>.Success(changed);
    }
}