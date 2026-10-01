namespace AtlasOps.Features.Data.DataPipelineOptimization;

public sealed record UpdateDataPipelineOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);