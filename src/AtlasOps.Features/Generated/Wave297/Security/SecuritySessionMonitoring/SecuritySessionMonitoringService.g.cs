namespace AtlasOps.Features.Security.SecuritySessionMonitoring;

using AtlasOps.Features;

public sealed class SecuritySessionMonitoringService(
    IAtlasOpsCapabilityRepository<SecuritySessionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecuritySessionMonitoringValidator validator = new();
    private readonly SecuritySessionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecuritySessionMonitoringChanged>> ExecuteAsync(
        UpdateSecuritySessionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecuritySessionMonitoringChanged>.Invalid(issues);
        }

        SecuritySessionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecuritySessionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecuritySessionMonitoringChanged>.Invalid(
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

        SecuritySessionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecuritySessionMonitoringChanged>.Success(changed);
    }
}