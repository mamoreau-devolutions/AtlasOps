namespace AtlasOps.Features.Editor.ScriptLibrary;

public sealed record ScriptLibraryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);