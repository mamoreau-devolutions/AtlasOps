namespace AtlasOps.Features.Compute.VirtualMachineOptimization;

public sealed record UpdateVirtualMachineOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);