namespace AtlasOps.Features.Security.SecuritySessionProvisioning;

public sealed record SecuritySessionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);