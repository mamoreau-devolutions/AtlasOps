namespace AtlasOps.Features.Edge.EdgeUpdateMonitoring;

using AtlasOps.Features;

public sealed class EdgeUpdateMonitoringService(
    IAtlasOpsCapabilityRepository<EdgeUpdateMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeUpdateMonitoringValidator validator = new();
    private readonly EdgeUpdateMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeUpdateMonitoringChanged>> ExecuteAsync(
        UpdateEdgeUpdateMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeUpdateMonitoringChanged>.Invalid(issues);
        }

        EdgeUpdateMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeUpdateMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeUpdateMonitoringChanged>.Invalid(
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

        EdgeUpdateMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeUpdateMonitoringChanged>.Success(changed);
    }
}