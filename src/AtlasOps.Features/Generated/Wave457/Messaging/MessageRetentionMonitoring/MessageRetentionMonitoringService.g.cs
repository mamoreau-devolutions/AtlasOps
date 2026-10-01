namespace AtlasOps.Features.Messaging.MessageRetentionMonitoring;

using AtlasOps.Features;

public sealed class MessageRetentionMonitoringService(
    IAtlasOpsCapabilityRepository<MessageRetentionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageRetentionMonitoringValidator validator = new();
    private readonly MessageRetentionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageRetentionMonitoringChanged>> ExecuteAsync(
        UpdateMessageRetentionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageRetentionMonitoringChanged>.Invalid(issues);
        }

        MessageRetentionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageRetentionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageRetentionMonitoringChanged>.Invalid(
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

        MessageRetentionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageRetentionMonitoringChanged>.Success(changed);
    }
}