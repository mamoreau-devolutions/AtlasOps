namespace AtlasOps.Features.Compute.ComputePatchProvisioning;

public sealed record UpdateComputePatchProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);