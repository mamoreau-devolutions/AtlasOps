namespace AtlasOps.Features.Data.DataLineageProvisioning;

public sealed record UpdateDataLineageProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);