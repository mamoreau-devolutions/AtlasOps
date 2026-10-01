namespace AtlasOps.Features.Data.DataTransformRecovery;

public sealed record UpdateDataTransformRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);