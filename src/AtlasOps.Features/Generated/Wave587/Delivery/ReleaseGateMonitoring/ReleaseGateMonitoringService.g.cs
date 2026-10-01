namespace AtlasOps.Features.Delivery.ReleaseGateMonitoring;

using AtlasOps.Features;

public sealed class ReleaseGateMonitoringService(
    IAtlasOpsCapabilityRepository<ReleaseGateMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseGateMonitoringValidator validator = new();
    private readonly ReleaseGateMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseGateMonitoringChanged>> ExecuteAsync(
        UpdateReleaseGateMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseGateMonitoringChanged>.Invalid(issues);
        }

        ReleaseGateMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseGateMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseGateMonitoringChanged>.Invalid(
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

        ReleaseGateMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseGateMonitoringChanged>.Success(changed);
    }
}