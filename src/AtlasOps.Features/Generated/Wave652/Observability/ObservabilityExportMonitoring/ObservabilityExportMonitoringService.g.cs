namespace AtlasOps.Features.Observability.ObservabilityExportMonitoring;

using AtlasOps.Features;

public sealed class ObservabilityExportMonitoringService(
    IAtlasOpsCapabilityRepository<ObservabilityExportMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilityExportMonitoringValidator validator = new();
    private readonly ObservabilityExportMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilityExportMonitoringChanged>> ExecuteAsync(
        UpdateObservabilityExportMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilityExportMonitoringChanged>.Invalid(issues);
        }

        ObservabilityExportMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilityExportMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilityExportMonitoringChanged>.Invalid(
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

        ObservabilityExportMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilityExportMonitoringChanged>.Success(changed);
    }
}