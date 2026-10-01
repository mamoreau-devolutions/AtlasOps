namespace AtlasOps.Features.Compute.ComputeScheduleMonitoring;

using AtlasOps.Features;

public sealed class ComputeScheduleMonitoringService(
    IAtlasOpsCapabilityRepository<ComputeScheduleMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ComputeScheduleMonitoringValidator validator = new();
    private readonly ComputeScheduleMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ComputeScheduleMonitoringChanged>> ExecuteAsync(
        UpdateComputeScheduleMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ComputeScheduleMonitoringChanged>.Invalid(issues);
        }

        ComputeScheduleMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ComputeScheduleMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ComputeScheduleMonitoringChanged>.Invalid(
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

        ComputeScheduleMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ComputeScheduleMonitoringChanged>.Success(changed);
    }
}