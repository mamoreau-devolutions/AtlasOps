namespace AtlasOps.Features.Delivery.ReleaseCalendarMonitoring;

using AtlasOps.Features;

public sealed class ReleaseCalendarMonitoringService(
    IAtlasOpsCapabilityRepository<ReleaseCalendarMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseCalendarMonitoringValidator validator = new();
    private readonly ReleaseCalendarMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseCalendarMonitoringChanged>> ExecuteAsync(
        UpdateReleaseCalendarMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseCalendarMonitoringChanged>.Invalid(issues);
        }

        ReleaseCalendarMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseCalendarMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseCalendarMonitoringChanged>.Invalid(
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

        ReleaseCalendarMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseCalendarMonitoringChanged>.Success(changed);
    }
}