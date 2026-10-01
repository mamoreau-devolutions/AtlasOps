namespace AtlasOps.Features.Data.DataRetentionProvisioning;

public sealed record UpdateDataRetentionProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);