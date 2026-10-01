namespace AtlasOps.Features.Network.NetworkRouteMonitoring;

using AtlasOps.Features;

public sealed class NetworkRouteMonitoringService(
    IAtlasOpsCapabilityRepository<NetworkRouteMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkRouteMonitoringValidator validator = new();
    private readonly NetworkRouteMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkRouteMonitoringChanged>> ExecuteAsync(
        UpdateNetworkRouteMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkRouteMonitoringChanged>.Invalid(issues);
        }

        NetworkRouteMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkRouteMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkRouteMonitoringChanged>.Invalid(
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

        NetworkRouteMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkRouteMonitoringChanged>.Success(changed);
    }
}