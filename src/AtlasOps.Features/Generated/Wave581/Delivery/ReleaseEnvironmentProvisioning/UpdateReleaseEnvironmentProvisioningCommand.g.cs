namespace AtlasOps.Features.Delivery.ReleaseEnvironmentProvisioning;

public sealed record UpdateReleaseEnvironmentProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);