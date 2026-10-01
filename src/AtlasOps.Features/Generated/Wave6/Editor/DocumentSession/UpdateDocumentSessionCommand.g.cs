namespace AtlasOps.Features.Editor.DocumentSession;

public sealed record UpdateDocumentSessionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);