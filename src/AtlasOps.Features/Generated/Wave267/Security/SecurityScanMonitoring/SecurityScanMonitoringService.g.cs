namespace AtlasOps.Features.Security.SecurityScanMonitoring;

using AtlasOps.Features;

public sealed class SecurityScanMonitoringService(
    IAtlasOpsCapabilityRepository<SecurityScanMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityScanMonitoringValidator validator = new();
    private readonly SecurityScanMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityScanMonitoringChanged>> ExecuteAsync(
        UpdateSecurityScanMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityScanMonitoringChanged>.Invalid(issues);
        }

        SecurityScanMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityScanMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityScanMonitoringChanged>.Invalid(
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

        SecurityScanMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityScanMonitoringChanged>.Success(changed);
    }
}