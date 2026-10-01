namespace AtlasOps.Features.Compute.ComputeScheduleRecovery;

public sealed record UpdateComputeScheduleRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);