namespace AtlasOps.Features.Identity.IdentityRoleProvisioning;

public sealed record IdentityRoleProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);