namespace AtlasOps.Features.Security.SecurityIdentityMonitoring;

using AtlasOps.Features;

public sealed class SecurityIdentityMonitoringService(
    IAtlasOpsCapabilityRepository<SecurityIdentityMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityIdentityMonitoringValidator validator = new();
    private readonly SecurityIdentityMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityIdentityMonitoringChanged>> ExecuteAsync(
        UpdateSecurityIdentityMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityIdentityMonitoringChanged>.Invalid(issues);
        }

        SecurityIdentityMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityIdentityMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityIdentityMonitoringChanged>.Invalid(
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

        SecurityIdentityMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityIdentityMonitoringChanged>.Success(changed);
    }
}