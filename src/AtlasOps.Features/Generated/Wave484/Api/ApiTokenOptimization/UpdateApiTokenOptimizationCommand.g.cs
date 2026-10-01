namespace AtlasOps.Features.Api.ApiTokenOptimization;

public sealed record UpdateApiTokenOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);