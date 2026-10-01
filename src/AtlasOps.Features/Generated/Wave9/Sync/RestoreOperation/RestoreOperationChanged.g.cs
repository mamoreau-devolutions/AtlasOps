namespace AtlasOps.Features.Sync.RestoreOperation;

public sealed record RestoreOperationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);