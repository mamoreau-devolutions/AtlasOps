namespace AtlasOps.Features.Edge.EdgePolicyProvisioning;

public sealed record EdgePolicyProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);