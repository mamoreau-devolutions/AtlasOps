namespace AtlasOps.Features.Identity.IdentityApplicationProvisioning;

public sealed record IdentityApplicationProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);