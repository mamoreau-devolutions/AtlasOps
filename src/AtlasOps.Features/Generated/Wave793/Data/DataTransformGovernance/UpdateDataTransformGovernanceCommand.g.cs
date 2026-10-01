namespace AtlasOps.Features.Data.DataTransformGovernance;

public sealed record UpdateDataTransformGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);