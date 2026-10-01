namespace AtlasOps.Features.Api.ApiEndpointOptimization;

public sealed record UpdateApiEndpointOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);