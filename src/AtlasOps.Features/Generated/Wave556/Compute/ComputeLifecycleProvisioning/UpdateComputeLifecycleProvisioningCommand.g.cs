namespace AtlasOps.Features.Compute.ComputeLifecycleProvisioning;

public sealed record UpdateComputeLifecycleProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);