namespace AtlasOps.Features.Edge.EdgeIncidentMonitoring;

using AtlasOps.Features;

public sealed class EdgeIncidentMonitoringService(
    IAtlasOpsCapabilityRepository<EdgeIncidentMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeIncidentMonitoringValidator validator = new();
    private readonly EdgeIncidentMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeIncidentMonitoringChanged>> ExecuteAsync(
        UpdateEdgeIncidentMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeIncidentMonitoringChanged>.Invalid(issues);
        }

        EdgeIncidentMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeIncidentMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeIncidentMonitoringChanged>.Invalid(
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

        EdgeIncidentMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeIncidentMonitoringChanged>.Success(changed);
    }
}