namespace AtlasOps.Features.Sync.MergeStrategy;

public sealed record UpdateMergeStrategyCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);