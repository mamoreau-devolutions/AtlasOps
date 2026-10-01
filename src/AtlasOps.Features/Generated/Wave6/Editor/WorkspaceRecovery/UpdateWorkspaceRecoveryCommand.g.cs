namespace AtlasOps.Features.Editor.WorkspaceRecovery;

public sealed record UpdateWorkspaceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);