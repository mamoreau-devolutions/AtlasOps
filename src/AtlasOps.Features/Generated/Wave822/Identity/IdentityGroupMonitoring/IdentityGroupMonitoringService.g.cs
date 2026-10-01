namespace AtlasOps.Features.Identity.IdentityGroupMonitoring;

using AtlasOps.Features;

public sealed class IdentityGroupMonitoringService(
    IAtlasOpsCapabilityRepository<IdentityGroupMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly IdentityGroupMonitoringValidator validator = new();
    private readonly IdentityGroupMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<IdentityGroupMonitoringChanged>> ExecuteAsync(
        UpdateIdentityGroupMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<IdentityGroupMonitoringChanged>.Invalid(issues);
        }

        IdentityGroupMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new IdentityGroupMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<IdentityGroupMonitoringChanged>.Invalid(
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

        IdentityGroupMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<IdentityGroupMonitoringChanged>.Success(changed);
    }
}