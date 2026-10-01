namespace AtlasOps.Features.Storage.StorageArchiveProvisioning;

public sealed record StorageArchiveProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);