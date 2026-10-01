namespace AtlasOps.Features.Storage.ObjectBucketRecovery;

public sealed record ObjectBucketRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);