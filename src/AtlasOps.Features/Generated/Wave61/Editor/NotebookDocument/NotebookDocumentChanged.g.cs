namespace AtlasOps.Features.Editor.NotebookDocument;

public sealed record NotebookDocumentChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);