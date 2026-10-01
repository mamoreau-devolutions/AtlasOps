namespace AtlasOps.Features.Sync.ConflictDetection;

public sealed record ConflictDetectionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);