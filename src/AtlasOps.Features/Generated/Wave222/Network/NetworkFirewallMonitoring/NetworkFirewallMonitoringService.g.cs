namespace AtlasOps.Features.Network.NetworkFirewallMonitoring;

using AtlasOps.Features;

public sealed class NetworkFirewallMonitoringService(
    IAtlasOpsCapabilityRepository<NetworkFirewallMonitoringItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkFirewallMonitoringValidator validator = new();
    private readonly NetworkFirewallMonitoringPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkFirewallMonitoringChanged>> ExecuteAsync(
        UpdateNetworkFirewallMonitoringCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkFirewallMonitoringChanged>.Invalid(issues);
        }

        NetworkFirewallMonitoringItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkFirewallMonitoringItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkFirewallMonitoringChanged>.Invalid(
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

        NetworkFirewallMonitoringChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkFirewallMonitoringChanged>.Success(changed);
    }
}