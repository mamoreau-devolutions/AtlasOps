namespace AtlasOps.Features.ServiceManagement.ServiceScorecardGovernance;

public sealed record UpdateServiceScorecardGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);