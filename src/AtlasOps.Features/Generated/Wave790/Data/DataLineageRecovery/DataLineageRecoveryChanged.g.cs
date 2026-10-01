namespace AtlasOps.Features.Data.DataLineageRecovery;

public sealed record DataLineageRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);