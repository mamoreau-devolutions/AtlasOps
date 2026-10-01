namespace AtlasOps.Features.Storage.BlockVolumeRecovery;

public sealed record UpdateBlockVolumeRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);