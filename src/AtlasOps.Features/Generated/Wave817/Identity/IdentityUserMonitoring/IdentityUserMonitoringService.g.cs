namespace AtlasOps.Features.Identity.IdentityUserMonitoring;

using AtlasOps.Features;

public sealed class IdentityUserMonitoringService(
    IAtlasOpsCapabilityRepository<IdentityUserMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityUserMonitoringValidator validator = new();
    private readonly IdentityUserMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityUserMonitoringChanged>> ExecuteAsync(
        UpdateIdentityUserMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityUserMonitoringChanged>.Invalid(issues);
        }

        IdentityUserMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityUserMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityUserMonitoringChanged>.Invalid(
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

        IdentityUserMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityUserMonitoringChanged>.Success(changed);
    }
}