namespace AtlasOps.Features.Messaging.MessageConsumerMonitoring;

using AtlasOps.Features;

public sealed class MessageConsumerMonitoringService(
    IAtlasOpsCapabilityRepository<MessageConsumerMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageConsumerMonitoringValidator validator = new();
    private readonly MessageConsumerMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageConsumerMonitoringChanged>> ExecuteAsync(
        UpdateMessageConsumerMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageConsumerMonitoringChanged>.Invalid(issues);
        }

        MessageConsumerMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageConsumerMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageConsumerMonitoringChanged>.Invalid(
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

        MessageConsumerMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageConsumerMonitoringChanged>.Success(changed);
    }
}