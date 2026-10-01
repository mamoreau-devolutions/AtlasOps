namespace AtlasOps.Features.Security.SecurityBoundaryProvisioning;

public sealed record SecurityBoundaryProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);