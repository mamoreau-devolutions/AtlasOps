namespace AtlasOps.Features.Identity.IdentitySessionProvisioning;

public sealed record IdentitySessionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);