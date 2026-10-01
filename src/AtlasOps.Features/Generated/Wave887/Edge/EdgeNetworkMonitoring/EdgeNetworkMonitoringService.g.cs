namespace AtlasOps.Features.Edge.EdgeNetworkMonitoring;

using AtlasOps.Features;

public sealed class EdgeNetworkMonitoringService(
    IAtlasOpsCapabilityRepository<EdgeNetworkMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeNetworkMonitoringValidator validator = new();
    private readonly EdgeNetworkMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeNetworkMonitoringChanged>> ExecuteAsync(
        UpdateEdgeNetworkMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeNetworkMonitoringChanged>.Invalid(issues);
        }

        EdgeNetworkMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeNetworkMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeNetworkMonitoringChanged>.Invalid(
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

        EdgeNetworkMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeNetworkMonitoringChanged>.Success(changed);
    }
}