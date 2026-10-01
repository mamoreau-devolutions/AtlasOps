namespace AtlasOps.Features.Messaging.MessageSchemaMonitoring;

using AtlasOps.Features;

public sealed class MessageSchemaMonitoringService(
    IAtlasOpsCapabilityRepository<MessageSchemaMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MessageSchemaMonitoringValidator validator = new();
    private readonly MessageSchemaMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MessageSchemaMonitoringChanged>> ExecuteAsync(
        UpdateMessageSchemaMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MessageSchemaMonitoringChanged>.Invalid(issues);
        }

        MessageSchemaMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MessageSchemaMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MessageSchemaMonitoringChanged>.Invalid(
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

        MessageSchemaMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MessageSchemaMonitoringChanged>.Success(changed);
    }
}