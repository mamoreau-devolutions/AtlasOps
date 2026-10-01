namespace AtlasOps.Features.Data.DataPipelineProvisioning;

public sealed record UpdateDataPipelineProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);