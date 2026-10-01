namespace AtlasOps.Features.Compute.ComputePatchOptimization;

using AtlasOps.Features;

public sealed class ComputePatchOptimizationService(
    IAtlasOpsCapabilityRepository<ComputePatchOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputePatchOptimizationValidator validator = new();
    private readonly ComputePatchOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputePatchOptimizationChanged>> ExecuteAsync(
        UpdateComputePatchOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputePatchOptimizationChanged>.Invalid(issues);
        }

        ComputePatchOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputePatchOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputePatchOptimizationChanged>.Invalid(
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

        ComputePatchOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputePatchOptimizationChanged>.Success(changed);
    }
}