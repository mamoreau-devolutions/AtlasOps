namespace AtlasOps.Features.Data.DataDatasetGovernance;

public sealed record UpdateDataDatasetGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);