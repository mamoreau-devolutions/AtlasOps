namespace AtlasOps.Features.Edge.EdgeDeploymentOptimization;

using AtlasOps.Features;

public sealed class EdgeDeploymentOptimizationService(
    IAtlasOpsCapabilityRepository<EdgeDeploymentOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeDeploymentOptimizationValidator validator = new();
    private readonly EdgeDeploymentOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeDeploymentOptimizationChanged>> ExecuteAsync(
        UpdateEdgeDeploymentOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeDeploymentOptimizationChanged>.Invalid(issues);
        }

        EdgeDeploymentOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeDeploymentOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeDeploymentOptimizationChanged>.Invalid(
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

        EdgeDeploymentOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeDeploymentOptimizationChanged>.Success(changed);
    }
}