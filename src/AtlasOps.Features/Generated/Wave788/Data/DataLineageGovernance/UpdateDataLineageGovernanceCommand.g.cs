namespace AtlasOps.Features.Data.DataLineageGovernance;

public sealed record UpdateDataLineageGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);