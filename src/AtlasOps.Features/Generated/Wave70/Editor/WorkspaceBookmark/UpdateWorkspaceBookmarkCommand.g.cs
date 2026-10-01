namespace AtlasOps.Features.Editor.WorkspaceBookmark;

public sealed record UpdateWorkspaceBookmarkCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);