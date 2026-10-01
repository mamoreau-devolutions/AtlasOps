namespace AtlasOps.Features.Delivery.ReleaseRollbackProvisioning;

public sealed record ReleaseRollbackProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);