namespace AtlasOps.Features.Security.SecurityBoundaryProvisioning;

public sealed record UpdateSecurityBoundaryProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);