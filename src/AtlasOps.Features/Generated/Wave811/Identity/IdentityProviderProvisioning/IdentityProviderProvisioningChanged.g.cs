namespace AtlasOps.Features.Identity.IdentityProviderProvisioning;

public sealed record IdentityProviderProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);