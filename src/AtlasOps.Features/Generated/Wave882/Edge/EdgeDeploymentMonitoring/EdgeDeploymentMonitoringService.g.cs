namespace AtlasOps.Features.Edge.EdgeDeploymentMonitoring;

using AtlasOps.Features;

public sealed class EdgeDeploymentMonitoringService(
    IAtlasOpsCapabilityRepository<EdgeDeploymentMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeDeploymentMonitoringValidator validator = new();
    private readonly EdgeDeploymentMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeDeploymentMonitoringChanged>> ExecuteAsync(
        UpdateEdgeDeploymentMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeDeploymentMonitoringChanged>.Invalid(issues);
        }

        EdgeDeploymentMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeDeploymentMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeDeploymentMonitoringChanged>.Invalid(
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

        EdgeDeploymentMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeDeploymentMonitoringChanged>.Success(changed);
    }
}