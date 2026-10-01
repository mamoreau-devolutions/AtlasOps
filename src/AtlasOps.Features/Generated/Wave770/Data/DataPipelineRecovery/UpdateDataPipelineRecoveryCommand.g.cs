namespace AtlasOps.Features.Data.DataPipelineRecovery;

public sealed record UpdateDataPipelineRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);