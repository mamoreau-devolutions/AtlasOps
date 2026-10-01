namespace AtlasOps.Features.Security.SecurityIdentityProvisioning;

public sealed record SecurityIdentityProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);