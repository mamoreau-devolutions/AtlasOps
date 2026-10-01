namespace AtlasOps.Features.Data.DataRetentionGovernance;

public sealed record UpdateDataRetentionGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);