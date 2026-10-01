namespace AtlasOps.Features.Data.DataQualityGovernance;

public sealed record UpdateDataQualityGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);