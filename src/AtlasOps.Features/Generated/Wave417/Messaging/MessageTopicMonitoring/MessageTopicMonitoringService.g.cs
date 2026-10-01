namespace AtlasOps.Features.Messaging.MessageTopicMonitoring;

using AtlasOps.Features;

public sealed class MessageTopicMonitoringService(
    IAtlasOpsCapabilityRepository<MessageTopicMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageTopicMonitoringValidator validator = new();
    private readonly MessageTopicMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageTopicMonitoringChanged>> ExecuteAsync(
        UpdateMessageTopicMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageTopicMonitoringChanged>.Invalid(issues);
        }

        MessageTopicMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageTopicMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageTopicMonitoringChanged>.Invalid(
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

        MessageTopicMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageTopicMonitoringChanged>.Success(changed);
    }
}