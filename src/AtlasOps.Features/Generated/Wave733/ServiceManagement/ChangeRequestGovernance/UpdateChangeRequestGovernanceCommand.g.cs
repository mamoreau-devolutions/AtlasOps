namespace AtlasOps.Features.ServiceManagement.ChangeRequestGovernance;

public sealed record UpdateChangeRequestGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);