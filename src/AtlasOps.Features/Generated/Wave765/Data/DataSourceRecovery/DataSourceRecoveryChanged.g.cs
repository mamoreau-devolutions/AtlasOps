namespace AtlasOps.Features.Data.DataSourceRecovery;

public sealed record DataSourceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);