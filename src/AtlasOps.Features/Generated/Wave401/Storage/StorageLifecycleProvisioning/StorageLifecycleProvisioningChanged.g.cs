namespace AtlasOps.Features.Storage.StorageLifecycleProvisioning;

public sealed record StorageLifecycleProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);