namespace AtlasOps.Features.Compute.ComputeReservationMonitoring;

using AtlasOps.Features;

public sealed class ComputeReservationMonitoringService(
    IAtlasOpsCapabilityRepository<ComputeReservationMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeReservationMonitoringValidator validator = new();
    private readonly ComputeReservationMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeReservationMonitoringChanged>> ExecuteAsync(
        UpdateComputeReservationMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeReservationMonitoringChanged>.Invalid(issues);
        }

        ComputeReservationMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeReservationMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeReservationMonitoringChanged>.Invalid(
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

        ComputeReservationMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeReservationMonitoringChanged>.Success(changed);
    }
}