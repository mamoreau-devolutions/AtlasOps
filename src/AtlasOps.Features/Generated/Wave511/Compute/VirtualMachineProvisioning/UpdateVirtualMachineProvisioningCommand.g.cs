namespace AtlasOps.Features.Compute.VirtualMachineProvisioning;

public sealed record UpdateVirtualMachineProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);