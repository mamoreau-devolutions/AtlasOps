namespace AtlasOps.Features.Edge.EdgeUpdateOptimization;

using AtlasOps.Features;

public sealed class EdgeUpdateOptimizationService(
    IAtlasOpsCapabilityRepository<EdgeUpdateOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeUpdateOptimizationValidator validator = new();
    private readonly EdgeUpdateOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeUpdateOptimizationChanged>> ExecuteAsync(
        UpdateEdgeUpdateOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeUpdateOptimizationChanged>.Invalid(issues);
        }

        EdgeUpdateOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeUpdateOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeUpdateOptimizationChanged>.Invalid(
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

        EdgeUpdateOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeUpdateOptimizationChanged>.Success(changed);
    }
}