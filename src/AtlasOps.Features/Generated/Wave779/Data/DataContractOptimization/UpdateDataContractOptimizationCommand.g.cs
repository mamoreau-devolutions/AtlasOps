namespace AtlasOps.Features.Data.DataContractOptimization;

public sealed record UpdateDataContractOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);