namespace AtlasOps.Features.Security.SecurityPatchMonitoring;

using AtlasOps.Features;

public sealed class SecurityPatchMonitoringService(
    IAtlasOpsCapabilityRepository<SecurityPatchMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly SecurityPatchMonitoringValidator validator = new();
    private readonly SecurityPatchMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<SecurityPatchMonitoringChanged>> ExecuteAsync(
        UpdateSecurityPatchMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<SecurityPatchMonitoringChanged>.Invalid(issues);
        }

        SecurityPatchMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new SecurityPatchMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<SecurityPatchMonitoringChanged>.Invalid(
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

        SecurityPatchMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<SecurityPatchMonitoringChanged>.Success(changed);
    }
}