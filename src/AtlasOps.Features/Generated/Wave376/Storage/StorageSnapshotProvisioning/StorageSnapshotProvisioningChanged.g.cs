namespace AtlasOps.Features.Storage.StorageSnapshotProvisioning;

public sealed record StorageSnapshotProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);