namespace AtlasOps.Features.Storage.FileShareRecovery;

public sealed record FileShareRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);