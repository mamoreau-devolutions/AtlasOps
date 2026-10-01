namespace AtlasOps.Features.Delivery.ReleaseEnvironmentMonitoring;

using AtlasOps.Features;

public sealed class ReleaseEnvironmentMonitoringService(
    IAtlasOpsCapabilityRepository<ReleaseEnvironmentMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseEnvironmentMonitoringValidator validator = new();
    private readonly ReleaseEnvironmentMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseEnvironmentMonitoringChanged>> ExecuteAsync(
        UpdateReleaseEnvironmentMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseEnvironmentMonitoringChanged>.Invalid(issues);
        }

        ReleaseEnvironmentMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseEnvironmentMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseEnvironmentMonitoringChanged>.Invalid(
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

        ReleaseEnvironmentMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseEnvironmentMonitoringChanged>.Success(changed);
    }
}