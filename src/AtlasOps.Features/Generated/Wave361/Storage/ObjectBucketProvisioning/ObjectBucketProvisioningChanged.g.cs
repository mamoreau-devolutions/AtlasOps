namespace AtlasOps.Features.Storage.ObjectBucketProvisioning;

public sealed record ObjectBucketProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);