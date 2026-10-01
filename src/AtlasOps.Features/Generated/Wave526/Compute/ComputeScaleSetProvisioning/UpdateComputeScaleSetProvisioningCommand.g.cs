namespace AtlasOps.Features.Compute.ComputeScaleSetProvisioning;

public sealed record UpdateComputeScaleSetProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);