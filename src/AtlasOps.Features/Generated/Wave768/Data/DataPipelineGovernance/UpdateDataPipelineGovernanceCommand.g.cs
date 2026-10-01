namespace AtlasOps.Features.Data.DataPipelineGovernance;

public sealed record UpdateDataPipelineGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);