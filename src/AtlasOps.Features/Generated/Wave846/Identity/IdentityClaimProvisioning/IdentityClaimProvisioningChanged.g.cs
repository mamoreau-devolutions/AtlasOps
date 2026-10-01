namespace AtlasOps.Features.Identity.IdentityClaimProvisioning;

public sealed record IdentityClaimProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);