namespace AtlasOps.Features.Data.DataAccessRecovery;

public sealed record DataAccessRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);