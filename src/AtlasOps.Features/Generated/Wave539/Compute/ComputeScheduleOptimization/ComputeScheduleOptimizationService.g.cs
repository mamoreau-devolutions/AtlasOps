namespace AtlasOps.Features.Compute.ComputeScheduleOptimization;

using AtlasOps.Features;

public sealed class ComputeScheduleOptimizationService(
    IAtlasOpsCapabilityRepository<ComputeScheduleOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeScheduleOptimizationValidator validator = new();
    private readonly ComputeScheduleOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeScheduleOptimizationChanged>> ExecuteAsync(
        UpdateComputeScheduleOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeScheduleOptimizationChanged>.Invalid(issues);
        }

        ComputeScheduleOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeScheduleOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeScheduleOptimizationChanged>.Invalid(
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

        ComputeScheduleOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeScheduleOptimizationChanged>.Success(changed);
    }
}