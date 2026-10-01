namespace AtlasOps.Features.Storage.FileShareRecovery;

public sealed record UpdateFileShareRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);