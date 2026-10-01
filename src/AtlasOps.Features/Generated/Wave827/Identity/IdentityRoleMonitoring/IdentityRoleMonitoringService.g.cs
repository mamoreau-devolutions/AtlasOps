namespace AtlasOps.Features.Identity.IdentityRoleMonitoring;

using AtlasOps.Features;

public sealed class IdentityRoleMonitoringService(
    IAtlasOpsCapabilityRepository<IdentityRoleMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityRoleMonitoringValidator validator = new();
    private readonly IdentityRoleMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityRoleMonitoringChanged>> ExecuteAsync(
        UpdateIdentityRoleMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityRoleMonitoringChanged>.Invalid(issues);
        }

        IdentityRoleMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityRoleMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityRoleMonitoringChanged>.Invalid(
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

        IdentityRoleMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityRoleMonitoringChanged>.Success(changed);
    }
}