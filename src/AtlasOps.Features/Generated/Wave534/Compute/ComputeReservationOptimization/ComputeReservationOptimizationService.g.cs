namespace AtlasOps.Features.Compute.ComputeReservationOptimization;

using AtlasOps.Features;

public sealed class ComputeReservationOptimizationService(
    IAtlasOpsCapabilityRepository<ComputeReservationOptimizationItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeReservationOptimizationValidator validator = new();
    private readonly ComputeReservationOptimizationPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeReservationOptimizationChanged>> ExecuteAsync(
        UpdateComputeReservationOptimizationCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeReservationOptimizationChanged>.Invalid(issues);
        }

        ComputeReservationOptimizationItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeReservationOptimizationItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeReservationOptimizationChanged>.Invalid(
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

        ComputeReservationOptimizationChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeReservationOptimizationChanged>.Success(changed);
    }
}