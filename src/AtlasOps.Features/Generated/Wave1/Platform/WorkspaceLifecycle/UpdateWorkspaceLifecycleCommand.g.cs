namespace AtlasOps.Features.Platform.WorkspaceLifecycle;

public sealed record UpdateWorkspaceLifecycleCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);