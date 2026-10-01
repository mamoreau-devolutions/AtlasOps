namespace AtlasOps.Features.Analytics.SharedFilter;

public sealed record UpdateSharedFilterCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);