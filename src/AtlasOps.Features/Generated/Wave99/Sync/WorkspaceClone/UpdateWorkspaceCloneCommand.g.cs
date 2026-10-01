namespace AtlasOps.Features.Sync.WorkspaceClone;

public sealed record UpdateWorkspaceCloneCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);