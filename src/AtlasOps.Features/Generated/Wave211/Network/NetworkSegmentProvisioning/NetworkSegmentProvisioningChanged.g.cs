namespace AtlasOps.Features.Network.NetworkSegmentProvisioning;

public sealed record NetworkSegmentProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);