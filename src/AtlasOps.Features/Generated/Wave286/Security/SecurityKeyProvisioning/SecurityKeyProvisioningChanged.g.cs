namespace AtlasOps.Features.Security.SecurityKeyProvisioning;

public sealed record SecurityKeyProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);