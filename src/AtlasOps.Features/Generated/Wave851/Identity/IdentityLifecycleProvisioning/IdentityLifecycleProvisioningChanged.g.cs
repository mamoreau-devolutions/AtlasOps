namespace AtlasOps.Features.Identity.IdentityLifecycleProvisioning;

public sealed record IdentityLifecycleProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);