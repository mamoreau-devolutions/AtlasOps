namespace AtlasOps.Features.Sync.ConflictResolution;

public sealed record ConflictResolutionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);