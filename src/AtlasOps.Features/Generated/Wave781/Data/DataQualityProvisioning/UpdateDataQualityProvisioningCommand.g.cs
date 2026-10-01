namespace AtlasOps.Features.Data.DataQualityProvisioning;

public sealed record UpdateDataQualityProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);