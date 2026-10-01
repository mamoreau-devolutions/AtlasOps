namespace AtlasOps.Features.Network.NetworkFirewallProvisioning;

public sealed record NetworkFirewallProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);