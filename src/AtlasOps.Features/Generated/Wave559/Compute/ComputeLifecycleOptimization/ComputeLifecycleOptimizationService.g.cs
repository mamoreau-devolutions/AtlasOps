namespace AtlasOps.Features.Compute.ComputeLifecycleOptimization;

using AtlasOps.Features;

public sealed class ComputeLifecycleOptimizationService(
    IAtlasOpsCapabilityRepository<ComputeLifecycleOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeLifecycleOptimizationValidator validator = new();
    private readonly ComputeLifecycleOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeLifecycleOptimizationChanged>> ExecuteAsync(
        UpdateComputeLifecycleOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeLifecycleOptimizationChanged>.Invalid(issues);
        }

        ComputeLifecycleOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeLifecycleOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeLifecycleOptimizationChanged>.Invalid(
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

        ComputeLifecycleOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeLifecycleOptimizationChanged>.Success(changed);
    }
}