namespace AtlasOps.Features.Compute.ComputeMetricProvisioning;

public sealed record UpdateComputeMetricProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);