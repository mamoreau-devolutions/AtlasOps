namespace AtlasOps.Features.Compute.VirtualMachineGovernance;

public sealed record UpdateVirtualMachineGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);