namespace AtlasOps.Features.Editor.DocumentSession;

public sealed record DocumentSessionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);