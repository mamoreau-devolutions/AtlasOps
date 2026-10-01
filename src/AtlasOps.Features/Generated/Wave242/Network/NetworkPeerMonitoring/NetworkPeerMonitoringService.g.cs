namespace AtlasOps.Features.Network.NetworkPeerMonitoring;

using AtlasOps.Features;

public sealed class NetworkPeerMonitoringService(
    IAtlasOpsCapabilityRepository<NetworkPeerMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkPeerMonitoringValidator validator = new();
    private readonly NetworkPeerMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkPeerMonitoringChanged>> ExecuteAsync(
        UpdateNetworkPeerMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkPeerMonitoringChanged>.Invalid(issues);
        }

        NetworkPeerMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkPeerMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkPeerMonitoringChanged>.Invalid(
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

        NetworkPeerMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkPeerMonitoringChanged>.Success(changed);
    }
}