namespace AtlasOps.Features.Delivery.ReleaseRollbackProvisioning;

public sealed record UpdateReleaseRollbackProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);