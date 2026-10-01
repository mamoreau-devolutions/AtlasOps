namespace AtlasOps.Features.Cloud.CloudIdentityProvisioning;

public sealed record CloudIdentityProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);