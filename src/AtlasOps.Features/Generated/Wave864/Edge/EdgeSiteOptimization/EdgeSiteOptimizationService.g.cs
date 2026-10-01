namespace AtlasOps.Features.Edge.EdgeSiteOptimization;

using AtlasOps.Features;

public sealed class EdgeSiteOptimizationService(
    IAtlasOpsCapabilityRepository<EdgeSiteOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeSiteOptimizationValidator validator = new();
    private readonly EdgeSiteOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeSiteOptimizationChanged>> ExecuteAsync(
        UpdateEdgeSiteOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeSiteOptimizationChanged>.Invalid(issues);
        }

        EdgeSiteOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeSiteOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeSiteOptimizationChanged>.Invalid(
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

        EdgeSiteOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeSiteOptimizationChanged>.Success(changed);
    }
}