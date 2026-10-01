namespace AtlasOps.Features.Cloud.GcpProjectMonitoring;

using AtlasOps.Features;

public sealed class GcpProjectMonitoringService(
    IAtlasOpsCapabilityRepository<GcpProjectMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly GcpProjectMonitoringValidator validator = new();
    private readonly GcpProjectMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<GcpProjectMonitoringChanged>> ExecuteAsync(
        UpdateGcpProjectMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<GcpProjectMonitoringChanged>.Invalid(issues);
        }

        GcpProjectMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new GcpProjectMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<GcpProjectMonitoringChanged>.Invalid(
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

        GcpProjectMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<GcpProjectMonitoringChanged>.Success(changed);
    }
}