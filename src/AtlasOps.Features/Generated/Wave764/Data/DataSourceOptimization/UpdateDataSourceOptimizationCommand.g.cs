namespace AtlasOps.Features.Data.DataSourceOptimization;

public sealed record UpdateDataSourceOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);