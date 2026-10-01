namespace AtlasOps.Features.Database.DatabaseQueryRecovery;

public sealed record DatabaseQueryRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);