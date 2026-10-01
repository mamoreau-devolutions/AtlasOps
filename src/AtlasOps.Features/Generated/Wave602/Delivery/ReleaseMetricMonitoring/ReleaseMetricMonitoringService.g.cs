namespace AtlasOps.Features.Delivery.ReleaseMetricMonitoring;

using AtlasOps.Features;

public sealed class ReleaseMetricMonitoringService(
    IAtlasOpsCapabilityRepository<ReleaseMetricMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseMetricMonitoringValidator validator = new();
    private readonly ReleaseMetricMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseMetricMonitoringChanged>> ExecuteAsync(
        UpdateReleaseMetricMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseMetricMonitoringChanged>.Invalid(issues);
        }

        ReleaseMetricMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseMetricMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseMetricMonitoringChanged>.Invalid(
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

        ReleaseMetricMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseMetricMonitoringChanged>.Success(changed);
    }
}