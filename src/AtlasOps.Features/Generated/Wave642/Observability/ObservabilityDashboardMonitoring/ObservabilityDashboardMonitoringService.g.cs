namespace AtlasOps.Features.Observability.ObservabilityDashboardMonitoring;

using AtlasOps.Features;

public sealed class ObservabilityDashboardMonitoringService(
    IAtlasOpsCapabilityRepository<ObservabilityDashboardMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilityDashboardMonitoringValidator validator = new();
    private readonly ObservabilityDashboardMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilityDashboardMonitoringChanged>> ExecuteAsync(
        UpdateObservabilityDashboardMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilityDashboardMonitoringChanged>.Invalid(issues);
        }

        ObservabilityDashboardMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilityDashboardMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilityDashboardMonitoringChanged>.Invalid(
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

        ObservabilityDashboardMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilityDashboardMonitoringChanged>.Success(changed);
    }
}