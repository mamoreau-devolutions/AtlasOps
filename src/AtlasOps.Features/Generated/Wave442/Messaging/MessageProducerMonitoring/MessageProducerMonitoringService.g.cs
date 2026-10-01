namespace AtlasOps.Features.Messaging.MessageProducerMonitoring;

using AtlasOps.Features;

public sealed class MessageProducerMonitoringService(
    IAtlasOpsCapabilityRepository<MessageProducerMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageProducerMonitoringValidator validator = new();
    private readonly MessageProducerMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageProducerMonitoringChanged>> ExecuteAsync(
        UpdateMessageProducerMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageProducerMonitoringChanged>.Invalid(issues);
        }

        MessageProducerMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageProducerMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageProducerMonitoringChanged>.Invalid(
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

        MessageProducerMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageProducerMonitoringChanged>.Success(changed);
    }
}