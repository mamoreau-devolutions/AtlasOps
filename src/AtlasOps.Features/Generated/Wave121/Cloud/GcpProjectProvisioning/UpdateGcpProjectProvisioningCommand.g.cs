namespace AtlasOps.Features.Cloud.GcpProjectProvisioning;

public sealed record UpdateGcpProjectProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);