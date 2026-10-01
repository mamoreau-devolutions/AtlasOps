namespace AtlasOps.Features.Compute.ComputeScaleSetOptimization;

using AtlasOps.Features;

public sealed class ComputeScaleSetOptimizationService(
    IAtlasOpsCapabilityRepository<ComputeScaleSetOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeScaleSetOptimizationValidator validator = new();
    private readonly ComputeScaleSetOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeScaleSetOptimizationChanged>> ExecuteAsync(
        UpdateComputeScaleSetOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeScaleSetOptimizationChanged>.Invalid(issues);
        }

        ComputeScaleSetOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeScaleSetOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeScaleSetOptimizationChanged>.Invalid(
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

        ComputeScaleSetOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeScaleSetOptimizationChanged>.Success(changed);
    }
}