namespace AtlasOps.Features.Editor.QueryExecution;

public sealed record QueryExecutionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);