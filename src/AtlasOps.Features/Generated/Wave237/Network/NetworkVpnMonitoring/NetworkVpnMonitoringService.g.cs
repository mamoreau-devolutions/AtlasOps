namespace AtlasOps.Features.Network.NetworkVpnMonitoring;

using AtlasOps.Features;

public sealed class NetworkVpnMonitoringService(
    IAtlasOpsCapabilityRepository<NetworkVpnMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkVpnMonitoringValidator validator = new();
    private readonly NetworkVpnMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkVpnMonitoringChanged>> ExecuteAsync(
        UpdateNetworkVpnMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkVpnMonitoringChanged>.Invalid(issues);
        }

        NetworkVpnMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkVpnMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkVpnMonitoringChanged>.Invalid(
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

        NetworkVpnMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkVpnMonitoringChanged>.Success(changed);
    }
}