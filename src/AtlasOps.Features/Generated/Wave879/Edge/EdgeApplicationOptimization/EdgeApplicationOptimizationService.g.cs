namespace AtlasOps.Features.Edge.EdgeApplicationOptimization;

using AtlasOps.Features;

public sealed class EdgeApplicationOptimizationService(
    IAtlasOpsCapabilityRepository<EdgeApplicationOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeApplicationOptimizationValidator validator = new();
    private readonly EdgeApplicationOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeApplicationOptimizationChanged>> ExecuteAsync(
        UpdateEdgeApplicationOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeApplicationOptimizationChanged>.Invalid(issues);
        }

        EdgeApplicationOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeApplicationOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeApplicationOptimizationChanged>.Invalid(
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

        EdgeApplicationOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeApplicationOptimizationChanged>.Success(changed);
    }
}