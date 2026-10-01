namespace AtlasOps.Features.Editor.ResultProjection;

public sealed record ResultProjectionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);