namespace AtlasOps.Features.Sync.RestoreOperation;

public sealed record UpdateRestoreOperationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);