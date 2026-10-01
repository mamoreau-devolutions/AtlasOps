namespace AtlasOps.Features.Cloud.CloudStorageProvisioning;

public sealed record CloudStorageProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);