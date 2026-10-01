namespace AtlasOps.Features.Editor.TerminalSession;

public sealed record TerminalSessionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);