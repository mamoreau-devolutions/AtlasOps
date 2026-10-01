namespace AtlasOps.Features.Delivery.ReleaseMetricProvisioning;

public sealed record UpdateReleaseMetricProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);