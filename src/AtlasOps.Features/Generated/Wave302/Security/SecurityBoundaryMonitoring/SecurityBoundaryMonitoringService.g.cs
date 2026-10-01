namespace AtlasOps.Features.Security.SecurityBoundaryMonitoring;

using AtlasOps.Features;

public sealed class SecurityBoundaryMonitoringService(
    IAtlasOpsCapabilityRepository<SecurityBoundaryMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityBoundaryMonitoringValidator validator = new();
    private readonly SecurityBoundaryMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityBoundaryMonitoringChanged>> ExecuteAsync(
        UpdateSecurityBoundaryMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityBoundaryMonitoringChanged>.Invalid(issues);
        }

        SecurityBoundaryMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityBoundaryMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityBoundaryMonitoringChanged>.Invalid(
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

        SecurityBoundaryMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityBoundaryMonitoringChanged>.Success(changed);
    }
}