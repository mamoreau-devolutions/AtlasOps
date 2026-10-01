namespace AtlasOps.Features.Security.SecurityScanProvisioning;

public sealed record SecurityScanProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);