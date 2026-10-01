namespace AtlasOps.Features.Delivery.ReleaseCalendarProvisioning;

public sealed record UpdateReleaseCalendarProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);