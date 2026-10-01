namespace AtlasOps.Features.Security.SecurityScanGovernance;

public sealed record UpdateSecurityScanGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);