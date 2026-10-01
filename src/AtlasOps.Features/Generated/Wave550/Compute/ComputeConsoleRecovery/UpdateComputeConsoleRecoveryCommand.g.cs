namespace AtlasOps.Features.Compute.ComputeConsoleRecovery;

public sealed record UpdateComputeConsoleRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);