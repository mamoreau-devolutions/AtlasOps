namespace AtlasOps.Features.Editor.NotebookDocument;

public sealed record UpdateNotebookDocumentCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);