namespace AtlasOps.Features.Data.DataRetentionRecovery;

public sealed record DataRetentionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);