namespace AtlasOps.Features.Storage.StorageTransferProvisioning;

public sealed record StorageTransferProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);