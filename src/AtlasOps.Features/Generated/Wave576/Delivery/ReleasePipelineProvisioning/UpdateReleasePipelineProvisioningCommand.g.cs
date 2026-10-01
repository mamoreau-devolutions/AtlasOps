namespace AtlasOps.Features.Delivery.ReleasePipelineProvisioning;

public sealed record UpdateReleasePipelineProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);