namespace AtlasOps.Features.Delivery.ReleaseApprovalMonitoring;

using AtlasOps.Features;

public sealed class ReleaseApprovalMonitoringService(
    IAtlasOpsCapabilityRepository<ReleaseApprovalMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly ReleaseApprovalMonitoringValidator validator = new();
    private readonly ReleaseApprovalMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<ReleaseApprovalMonitoringChanged>> ExecuteAsync(
        UpdateReleaseApprovalMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<ReleaseApprovalMonitoringChanged>.Invalid(issues);
        }

        ReleaseApprovalMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new ReleaseApprovalMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<ReleaseApprovalMonitoringChanged>.Invalid(
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

        ReleaseApprovalMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<ReleaseApprovalMonitoringChanged>.Success(changed);
    }
}