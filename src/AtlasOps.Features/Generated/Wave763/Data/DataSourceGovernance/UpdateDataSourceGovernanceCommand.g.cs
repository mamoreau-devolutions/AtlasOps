namespace AtlasOps.Features.Data.DataSourceGovernance;

public sealed record UpdateDataSourceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);