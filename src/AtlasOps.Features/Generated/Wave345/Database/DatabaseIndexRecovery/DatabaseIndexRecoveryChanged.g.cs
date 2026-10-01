namespace AtlasOps.Features.Database.DatabaseIndexRecovery;

public sealed record DatabaseIndexRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);