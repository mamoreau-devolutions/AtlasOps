namespace AtlasOps.Features.Compute.ComputeImageOptimization;

using AtlasOps.Features;

public sealed class ComputeImageOptimizationService(
    IAtlasOpsCapabilityRepository<ComputeImageOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeImageOptimizationValidator validator = new();
    private readonly ComputeImageOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeImageOptimizationChanged>> ExecuteAsync(
        UpdateComputeImageOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeImageOptimizationChanged>.Invalid(issues);
        }

        ComputeImageOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeImageOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeImageOptimizationChanged>.Invalid(
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

        ComputeImageOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeImageOptimizationChanged>.Success(changed);
    }
}