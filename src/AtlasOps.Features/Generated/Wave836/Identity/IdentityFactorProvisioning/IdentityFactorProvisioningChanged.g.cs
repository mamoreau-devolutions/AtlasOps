namespace AtlasOps.Features.Identity.IdentityFactorProvisioning;

public sealed record IdentityFactorProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);