namespace AtlasOps.Features.Data.DataAccessGovernance;

public sealed record UpdateDataAccessGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);