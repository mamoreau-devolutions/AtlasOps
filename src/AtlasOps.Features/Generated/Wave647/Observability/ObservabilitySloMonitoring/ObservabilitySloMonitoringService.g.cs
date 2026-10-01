namespace AtlasOps.Features.Observability.ObservabilitySloMonitoring;

using AtlasOps.Features;

public sealed class ObservabilitySloMonitoringService(
    IAtlasOpsCapabilityRepository<ObservabilitySloMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ObservabilitySloMonitoringValidator validator = new();
    private readonly ObservabilitySloMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ObservabilitySloMonitoringChanged>> ExecuteAsync(
        UpdateObservabilitySloMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ObservabilitySloMonitoringChanged>.Invalid(issues);
        }

        ObservabilitySloMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ObservabilitySloMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ObservabilitySloMonitoringChanged>.Invalid(
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

        ObservabilitySloMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ObservabilitySloMonitoringChanged>.Success(changed);
    }
}