namespace AtlasOps.Features.Compute.VirtualMachineRecovery;

public sealed record UpdateVirtualMachineRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);