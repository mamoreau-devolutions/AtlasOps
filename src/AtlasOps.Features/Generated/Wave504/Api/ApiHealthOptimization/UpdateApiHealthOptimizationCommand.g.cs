namespace AtlasOps.Features.Api.ApiHealthOptimization;

public sealed record UpdateApiHealthOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);