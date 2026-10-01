namespace AtlasOps.Features.Security.SecurityExceptionGovernance;

public sealed record UpdateSecurityExceptionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);