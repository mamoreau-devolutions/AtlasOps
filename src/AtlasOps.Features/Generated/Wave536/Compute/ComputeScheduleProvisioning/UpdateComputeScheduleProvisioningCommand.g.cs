namespace AtlasOps.Features.Compute.ComputeScheduleProvisioning;

public sealed record UpdateComputeScheduleProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);