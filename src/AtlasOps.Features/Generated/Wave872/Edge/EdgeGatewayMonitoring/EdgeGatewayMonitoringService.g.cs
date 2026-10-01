namespace AtlasOps.Features.Edge.EdgeGatewayMonitoring;

using AtlasOps.Features;

public sealed class EdgeGatewayMonitoringService(
    IAtlasOpsCapabilityRepository<EdgeGatewayMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly EdgeGatewayMonitoringValidator validator = new();
    private readonly EdgeGatewayMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<EdgeGatewayMonitoringChanged>> ExecuteAsync(
        UpdateEdgeGatewayMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<EdgeGatewayMonitoringChanged>.Invalid(issues);
        }

        EdgeGatewayMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new EdgeGatewayMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<EdgeGatewayMonitoringChanged>.Invalid(
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

        EdgeGatewayMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<EdgeGatewayMonitoringChanged>.Success(changed);
    }
}