namespace AtlasOps.Features.FinOps.CostAnomalyOptimization;

using AtlasOps.Features;

public sealed class CostAnomalyOptimizationService(
    IAtlasOpsCapabilityRepository<CostAnomalyOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostAnomalyOptimizationValidator validator = new();
    private readonly CostAnomalyOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostAnomalyOptimizationChanged>> ExecuteAsync(
        UpdateCostAnomalyOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostAnomalyOptimizationChanged>.Invalid(issues);
        }

        CostAnomalyOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostAnomalyOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostAnomalyOptimizationChanged>.Invalid(
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

        CostAnomalyOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostAnomalyOptimizationChanged>.Success(changed);
    }
}