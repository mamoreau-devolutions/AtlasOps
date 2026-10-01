namespace AtlasOps.Features.Identity.IdentityLifecycleMonitoring;

using AtlasOps.Features;

public sealed class IdentityLifecycleMonitoringService(
    IAtlasOpsCapabilityRepository<IdentityLifecycleMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityLifecycleMonitoringValidator validator = new();
    private readonly IdentityLifecycleMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityLifecycleMonitoringChanged>> ExecuteAsync(
        UpdateIdentityLifecycleMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityLifecycleMonitoringChanged>.Invalid(issues);
        }

        IdentityLifecycleMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityLifecycleMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityLifecycleMonitoringChanged>.Invalid(
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

        IdentityLifecycleMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityLifecycleMonitoringChanged>.Success(changed);
    }
}