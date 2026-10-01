namespace AtlasOps.Features.Inventory.DriftDetection;

public sealed record UpdateDriftDetectionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);