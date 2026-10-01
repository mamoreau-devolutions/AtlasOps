namespace AtlasOps.Features.Cloud.CloudDatabaseRecovery;

public sealed record CloudDatabaseRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);