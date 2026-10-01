namespace AtlasOps.Features.Edge.EdgeIncidentOptimization;

using AtlasOps.Features;

public sealed class EdgeIncidentOptimizationService(
    IAtlasOpsCapabilityRepository<EdgeIncidentOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeIncidentOptimizationValidator validator = new();
    private readonly EdgeIncidentOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeIncidentOptimizationChanged>> ExecuteAsync(
        UpdateEdgeIncidentOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeIncidentOptimizationChanged>.Invalid(issues);
        }

        EdgeIncidentOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeIncidentOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeIncidentOptimizationChanged>.Invalid(
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

        EdgeIncidentOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeIncidentOptimizationChanged>.Success(changed);
    }
}