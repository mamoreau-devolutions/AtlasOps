namespace AtlasOps.Features.BusinessContinuity.RecoverySiteProvisioning;

public sealed record UpdateRecoverySiteProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);