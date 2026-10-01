namespace AtlasOps.Features.Api.ApiClientOptimization;

public sealed record UpdateApiClientOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);