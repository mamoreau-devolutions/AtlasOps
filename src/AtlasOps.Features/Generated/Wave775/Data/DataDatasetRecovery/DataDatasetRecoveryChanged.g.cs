namespace AtlasOps.Features.Data.DataDatasetRecovery;

public sealed record DataDatasetRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);