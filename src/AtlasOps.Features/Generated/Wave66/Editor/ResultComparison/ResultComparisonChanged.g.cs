namespace AtlasOps.Features.Editor.ResultComparison;

public sealed record ResultComparisonChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);