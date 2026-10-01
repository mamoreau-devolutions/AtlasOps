namespace AtlasOps.Features.Database.DatabaseRestoreRecovery;

public sealed record DatabaseRestoreRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);