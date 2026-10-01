namespace AtlasOps.Features.Editor.TerminalProfile;

public sealed record TerminalProfileChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);