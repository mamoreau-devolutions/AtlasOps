namespace AtlasOps.Features.Edge.EdgeDeviceMonitoring;

using AtlasOps.Features;

public sealed class EdgeDeviceMonitoringService(
    IAtlasOpsCapabilityRepository<EdgeDeviceMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeDeviceMonitoringValidator validator = new();
    private readonly EdgeDeviceMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeDeviceMonitoringChanged>> ExecuteAsync(
        UpdateEdgeDeviceMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeDeviceMonitoringChanged>.Invalid(issues);
        }

        EdgeDeviceMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeDeviceMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeDeviceMonitoringChanged>.Invalid(
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

        EdgeDeviceMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeDeviceMonitoringChanged>.Success(changed);
    }
}