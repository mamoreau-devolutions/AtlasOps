namespace AtlasOps.Features.Sync.ConflictDetection;

public sealed record UpdateConflictDetectionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);