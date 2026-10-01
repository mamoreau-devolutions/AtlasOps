namespace AtlasOps.Features.Storage.StorageReplicationProvisioning;

public sealed record StorageReplicationProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);