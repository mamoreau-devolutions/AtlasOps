namespace AtlasOps.Features.Storage.StorageEncryptionProvisioning;

public sealed record StorageEncryptionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);