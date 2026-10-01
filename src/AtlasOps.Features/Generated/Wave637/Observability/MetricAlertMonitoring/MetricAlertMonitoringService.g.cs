namespace AtlasOps.Features.Observability.MetricAlertMonitoring;

using AtlasOps.Features;

public sealed class MetricAlertMonitoringService(
    IAtlasOpsCapabilityRepository<MetricAlertMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly MetricAlertMonitoringValidator validator = new();
    private readonly MetricAlertMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<MetricAlertMonitoringChanged>> ExecuteAsync(
        UpdateMetricAlertMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<MetricAlertMonitoringChanged>.Invalid(issues);
        }

        MetricAlertMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new MetricAlertMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<MetricAlertMonitoringChanged>.Invalid(
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

        MetricAlertMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<MetricAlertMonitoringChanged>.Success(changed);
    }
}