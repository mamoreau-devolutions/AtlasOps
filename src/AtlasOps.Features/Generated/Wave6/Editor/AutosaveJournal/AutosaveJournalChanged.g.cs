namespace AtlasOps.Features.Editor.AutosaveJournal;

public sealed record AutosaveJournalChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);