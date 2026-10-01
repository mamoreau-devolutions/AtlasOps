namespace AtlasOps.Features.Network.NetworkProbeProvisioning;

public sealed record NetworkProbeProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);