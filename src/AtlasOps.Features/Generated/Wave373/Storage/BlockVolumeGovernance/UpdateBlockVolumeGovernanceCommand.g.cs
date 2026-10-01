namespace AtlasOps.Features.Storage.BlockVolumeGovernance;

public sealed record UpdateBlockVolumeGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);