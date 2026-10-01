namespace AtlasOps.Features.Sync.ChangeVector;

public sealed record UpdateChangeVectorCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);