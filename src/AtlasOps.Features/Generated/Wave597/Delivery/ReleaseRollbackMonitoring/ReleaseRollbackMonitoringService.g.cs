namespace AtlasOps.Features.Delivery.ReleaseRollbackMonitoring;

using AtlasOps.Features;

public sealed class ReleaseRollbackMonitoringService(
    IAtlasOpsCapabilityRepository<ReleaseRollbackMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseRollbackMonitoringValidator validator = new();
    private readonly ReleaseRollbackMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseRollbackMonitoringChanged>> ExecuteAsync(
        UpdateReleaseRollbackMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseRollbackMonitoringChanged>.Invalid(issues);
        }

        ReleaseRollbackMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseRollbackMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseRollbackMonitoringChanged>.Invalid(
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

        ReleaseRollbackMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseRollbackMonitoringChanged>.Success(changed);
    }
}