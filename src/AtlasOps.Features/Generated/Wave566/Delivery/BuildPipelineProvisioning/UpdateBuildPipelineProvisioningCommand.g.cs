namespace AtlasOps.Features.Delivery.BuildPipelineProvisioning;

public sealed record UpdateBuildPipelineProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);