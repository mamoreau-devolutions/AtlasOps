namespace AtlasOps.Features.Observability.ObservabilityRetentionMonitoring;

using AtlasOps.Features;

public sealed class ObservabilityRetentionMonitoringService(
    IAtlasOpsCapabilityRepository<ObservabilityRetentionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilityRetentionMonitoringValidator validator = new();
    private readonly ObservabilityRetentionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilityRetentionMonitoringChanged>> ExecuteAsync(
        UpdateObservabilityRetentionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilityRetentionMonitoringChanged>.Invalid(issues);
        }

        ObservabilityRetentionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilityRetentionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilityRetentionMonitoringChanged>.Invalid(
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

        ObservabilityRetentionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilityRetentionMonitoringChanged>.Success(changed);
    }
}