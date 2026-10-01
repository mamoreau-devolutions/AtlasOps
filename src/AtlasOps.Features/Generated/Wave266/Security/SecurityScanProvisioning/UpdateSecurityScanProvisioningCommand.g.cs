namespace AtlasOps.Features.Security.SecurityScanProvisioning;

public sealed record UpdateSecurityScanProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);