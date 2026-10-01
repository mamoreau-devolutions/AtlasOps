namespace AtlasOps.Features.Edge.EdgeSiteMonitoring;

using AtlasOps.Features;

public sealed class EdgeSiteMonitoringService(
    IAtlasOpsCapabilityRepository<EdgeSiteMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeSiteMonitoringValidator validator = new();
    private readonly EdgeSiteMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeSiteMonitoringChanged>> ExecuteAsync(
        UpdateEdgeSiteMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeSiteMonitoringChanged>.Invalid(issues);
        }

        EdgeSiteMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeSiteMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeSiteMonitoringChanged>.Invalid(
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

        EdgeSiteMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeSiteMonitoringChanged>.Success(changed);
    }
}