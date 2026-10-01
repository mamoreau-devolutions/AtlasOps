namespace AtlasOps.Features.Edge.EdgeTelemetryMonitoring;

using AtlasOps.Features;

public sealed class EdgeTelemetryMonitoringService(
    IAtlasOpsCapabilityRepository<EdgeTelemetryMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeTelemetryMonitoringValidator validator = new();
    private readonly EdgeTelemetryMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeTelemetryMonitoringChanged>> ExecuteAsync(
        UpdateEdgeTelemetryMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeTelemetryMonitoringChanged>.Invalid(issues);
        }

        EdgeTelemetryMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeTelemetryMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeTelemetryMonitoringChanged>.Invalid(
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

        EdgeTelemetryMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeTelemetryMonitoringChanged>.Success(changed);
    }
}