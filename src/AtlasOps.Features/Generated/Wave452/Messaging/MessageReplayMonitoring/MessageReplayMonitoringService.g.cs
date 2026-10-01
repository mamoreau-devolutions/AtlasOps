namespace AtlasOps.Features.Messaging.MessageReplayMonitoring;

using AtlasOps.Features;

public sealed class MessageReplayMonitoringService(
    IAtlasOpsCapabilityRepository<MessageReplayMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageReplayMonitoringValidator validator = new();
    private readonly MessageReplayMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageReplayMonitoringChanged>> ExecuteAsync(
        UpdateMessageReplayMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageReplayMonitoringChanged>.Invalid(issues);
        }

        MessageReplayMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageReplayMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageReplayMonitoringChanged>.Invalid(
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

        MessageReplayMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageReplayMonitoringChanged>.Success(changed);
    }
}