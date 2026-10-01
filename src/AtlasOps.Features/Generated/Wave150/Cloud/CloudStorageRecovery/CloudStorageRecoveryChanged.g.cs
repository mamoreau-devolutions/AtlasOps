namespace AtlasOps.Features.Cloud.CloudStorageRecovery;

public sealed record CloudStorageRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);