namespace AtlasOps.Features.Sync.ConflictResolution;

public sealed record UpdateConflictResolutionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);