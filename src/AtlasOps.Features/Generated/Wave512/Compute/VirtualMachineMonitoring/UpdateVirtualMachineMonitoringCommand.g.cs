namespace AtlasOps.Features.Compute.VirtualMachineMonitoring;

public sealed record UpdateVirtualMachineMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);