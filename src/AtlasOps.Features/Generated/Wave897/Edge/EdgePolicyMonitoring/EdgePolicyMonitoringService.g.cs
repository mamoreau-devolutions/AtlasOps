namespace AtlasOps.Features.Edge.EdgePolicyMonitoring;

using AtlasOps.Features;

public sealed class EdgePolicyMonitoringService(
    IAtlasOpsCapabilityRepository<EdgePolicyMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgePolicyMonitoringValidator validator = new();
    private readonly EdgePolicyMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgePolicyMonitoringChanged>> ExecuteAsync(
        UpdateEdgePolicyMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgePolicyMonitoringChanged>.Invalid(issues);
        }

        EdgePolicyMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgePolicyMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgePolicyMonitoringChanged>.Invalid(
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

        EdgePolicyMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgePolicyMonitoringChanged>.Success(changed);
    }
}