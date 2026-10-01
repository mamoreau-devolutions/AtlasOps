namespace AtlasOps.Features.Edge.EdgeUpdateProvisioning;

public sealed record EdgeUpdateProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);