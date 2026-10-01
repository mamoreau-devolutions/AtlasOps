namespace AtlasOps.Features.Api.ApiVersionOptimization;

public sealed record UpdateApiVersionOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);