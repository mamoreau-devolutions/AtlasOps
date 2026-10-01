namespace AtlasOps.Features.Edge.EdgeApplicationProvisioning;

public sealed record EdgeApplicationProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);