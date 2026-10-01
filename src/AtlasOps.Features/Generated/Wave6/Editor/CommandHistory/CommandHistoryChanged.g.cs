namespace AtlasOps.Features.Editor.CommandHistory;

public sealed record CommandHistoryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);