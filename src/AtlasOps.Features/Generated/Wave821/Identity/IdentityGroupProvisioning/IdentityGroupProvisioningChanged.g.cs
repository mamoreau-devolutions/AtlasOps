namespace AtlasOps.Features.Identity.IdentityGroupProvisioning;

public sealed record IdentityGroupProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);