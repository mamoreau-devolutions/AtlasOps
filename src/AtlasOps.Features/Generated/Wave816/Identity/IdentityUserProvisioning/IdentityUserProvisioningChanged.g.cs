namespace AtlasOps.Features.Identity.IdentityUserProvisioning;

public sealed record IdentityUserProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);