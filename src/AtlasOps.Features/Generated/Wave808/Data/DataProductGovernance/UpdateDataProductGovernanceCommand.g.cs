namespace AtlasOps.Features.Data.DataProductGovernance;

public sealed record UpdateDataProductGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);