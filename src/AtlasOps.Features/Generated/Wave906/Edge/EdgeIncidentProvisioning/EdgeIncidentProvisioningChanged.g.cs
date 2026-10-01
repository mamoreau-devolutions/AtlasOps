namespace AtlasOps.Features.Edge.EdgeIncidentProvisioning;

public sealed record EdgeIncidentProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);