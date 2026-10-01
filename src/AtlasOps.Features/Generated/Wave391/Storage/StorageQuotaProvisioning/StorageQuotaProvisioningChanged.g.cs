namespace AtlasOps.Features.Storage.StorageQuotaProvisioning;

public sealed record StorageQuotaProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);