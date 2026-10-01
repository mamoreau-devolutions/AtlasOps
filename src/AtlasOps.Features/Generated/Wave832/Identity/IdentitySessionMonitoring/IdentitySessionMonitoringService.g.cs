namespace AtlasOps.Features.Identity.IdentitySessionMonitoring;

using AtlasOps.Features;

public sealed class IdentitySessionMonitoringService(
    IAtlasOpsCapabilityRepository<IdentitySessionMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentitySessionMonitoringValidator validator = new();
    private readonly IdentitySessionMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentitySessionMonitoringChanged>> ExecuteAsync(
        UpdateIdentitySessionMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentitySessionMonitoringChanged>.Invalid(issues);
        }

        IdentitySessionMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentitySessionMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentitySessionMonitoringChanged>.Invalid(
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

        IdentitySessionMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentitySessionMonitoringChanged>.Success(changed);
    }
}