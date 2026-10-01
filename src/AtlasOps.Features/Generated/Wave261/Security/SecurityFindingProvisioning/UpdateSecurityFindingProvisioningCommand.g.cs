namespace AtlasOps.Features.Security.SecurityFindingProvisioning;

public sealed record UpdateSecurityFindingProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);