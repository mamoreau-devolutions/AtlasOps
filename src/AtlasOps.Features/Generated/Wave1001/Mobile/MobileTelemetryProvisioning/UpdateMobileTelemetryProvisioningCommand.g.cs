namespace AtlasOps.Features.Mobile.MobileTelemetryProvisioning;

public sealed record UpdateMobileTelemetryProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);