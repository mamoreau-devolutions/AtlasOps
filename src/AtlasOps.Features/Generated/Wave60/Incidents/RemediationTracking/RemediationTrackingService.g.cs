namespace AtlasOps.Features.Incidents.RemediationTracking;

using AtlasOps.Features;

public sealed class RemediationTrackingService(
    IAtlasOpsCapabilityRepository<RemediationTrackingItem> repository,
    TimeProvider timeProvider)
{
    private readonly RemediationTrackingValidator validator = new();
    private readonly RemediationTrackingPolicy policy = new();

    public async Task<AtlasOpsOperationResult<RemediationTrackingChanged>> ExecuteAsync(
        UpdateRemediationTrackingCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<RemediationTrackingChanged>.Invalid(issues);
        }

        RemediationTrackingItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new RemediationTrackingItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<RemediationTrackingChanged>.Invalid(
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

        RemediationTrackingChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<RemediationTrackingChanged>.Success(changed);
    }
}