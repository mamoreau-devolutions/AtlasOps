namespace AtlasOps.Features.Data.DataDatasetProvisioning;

public sealed record UpdateDataDatasetProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);