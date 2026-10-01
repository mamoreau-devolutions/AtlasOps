namespace AtlasOps.Features.Security.SecurityBaselineProvisioning;

public sealed record UpdateSecurityBaselineProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);