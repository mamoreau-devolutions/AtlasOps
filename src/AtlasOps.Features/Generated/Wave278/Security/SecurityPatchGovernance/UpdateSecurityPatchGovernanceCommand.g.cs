namespace AtlasOps.Features.Security.SecurityPatchGovernance;

public sealed record UpdateSecurityPatchGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);