namespace AtlasOps.Features.Compute.ComputeImageProvisioning;

public sealed record UpdateComputeImageProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);