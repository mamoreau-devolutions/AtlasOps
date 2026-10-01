namespace AtlasOps.Features.Edge.EdgeNetworkProvisioning;

public sealed record EdgeNetworkProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);