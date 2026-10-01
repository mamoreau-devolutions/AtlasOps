namespace AtlasOps.Features.Network.NetworkFirewallProvisioning;

using AtlasOps.Features;

public sealed class NetworkFirewallProvisioningService(
    IAtlasOpsCapabilityRepository<NetworkFirewallProvisioningItem> repository,
    TimeProvider timeProvider)
{
    private readonly NetworkFirewallProvisioningValidator validator = new();
    private readonly NetworkFirewallProvisioningPolicy policy = new();

    public async Task<AtlasOpsOperationResult<NetworkFirewallProvisioningChanged>> ExecuteAsync(
        UpdateNetworkFirewallProvisioningCommand command,
        string actor,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<AtlasOpsValidationIssue> issues = this.validator.Validate(command);
        if (issues.Count > 0)
        {
            return AtlasOpsOperationResult<NetworkFirewallProvisioningChanged>.Invalid(issues);
        }

        NetworkFirewallProvisioningItem entity = await repository.GetAsync(command.Id, cancellationToken)
            ?? new NetworkFirewallProvisioningItem { Id = command.Id };
        string previousState = entity.State;

        if (!string.Equals(previousState, command.TargetState, StringComparison.OrdinalIgnoreCase) &&
            !this.policy.CanTransition(previousState, command.TargetState))
        {
            return AtlasOpsOperationResult<NetworkFirewallProvisioningChanged>.Invalid(
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

        NetworkFirewallProvisioningChanged changed = new(entity.Id, previousState, entity.State, actor, now);
        return AtlasOpsOperationResult<NetworkFirewallProvisioningChanged>.Success(changed);
    }
}