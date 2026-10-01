namespace AtlasOps.Features.Cloud.GcpProjectOptimization;

public sealed record UpdateGcpProjectOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);