namespace AtlasOps.Features.FinOps.CostAllocationOptimization;

using AtlasOps.Features;

public sealed class CostAllocationOptimizationService(
    IAtlasOpsCapabilityRepository<CostAllocationOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly CostAllocationOptimizationValidator validator = new();
    private readonly CostAllocationOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<CostAllocationOptimizationChanged>> ExecuteAsync(
        UpdateCostAllocationOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<CostAllocationOptimizationChanged>.Invalid(issues);
        }

        CostAllocationOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new CostAllocationOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<CostAllocationOptimizationChanged>.Invalid(
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

        CostAllocationOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<CostAllocationOptimizationChanged>.Success(changed);
    }
}