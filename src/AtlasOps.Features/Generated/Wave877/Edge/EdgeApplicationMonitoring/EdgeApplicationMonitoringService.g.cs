namespace AtlasOps.Features.Edge.EdgeApplicationMonitoring;

using AtlasOps.Features;

public sealed class EdgeApplicationMonitoringService(
    IAtlasOpsCapabilityRepository<EdgeApplicationMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeApplicationMonitoringValidator validator = new();
    private readonly EdgeApplicationMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeApplicationMonitoringChanged>> ExecuteAsync(
        UpdateEdgeApplicationMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeApplicationMonitoringChanged>.Invalid(issues);
        }

        EdgeApplicationMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeApplicationMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeApplicationMonitoringChanged>.Invalid(
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

        EdgeApplicationMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeApplicationMonitoringChanged>.Success(changed);
    }
}