namespace AtlasOps.Features.Editor.AutosaveJournal;

public sealed record UpdateAutosaveJournalCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);