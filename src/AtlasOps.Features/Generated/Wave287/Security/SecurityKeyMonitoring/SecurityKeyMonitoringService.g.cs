namespace AtlasOps.Features.Security.SecurityKeyMonitoring;

using AtlasOps.Features;

public sealed class SecurityKeyMonitoringService(
    IAtlasOpsCapabilityRepository<SecurityKeyMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityKeyMonitoringValidator validator = new();
    private readonly SecurityKeyMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityKeyMonitoringChanged>> ExecuteAsync(
        UpdateSecurityKeyMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityKeyMonitoringChanged>.Invalid(issues);
        }

        SecurityKeyMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityKeyMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityKeyMonitoringChanged>.Invalid(
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

        SecurityKeyMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityKeyMonitoringChanged>.Success(changed);
    }
}