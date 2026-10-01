namespace AtlasOps.Features.Identity.IdentityAuditProvisioning;

public sealed record IdentityAuditProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);