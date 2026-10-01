namespace AtlasOps.Features.Delivery.ReleaseGateProvisioning;

public sealed record UpdateReleaseGateProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);