namespace AtlasOps.Features.Edge.EdgeSiteProvisioning;

public sealed record EdgeSiteProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);