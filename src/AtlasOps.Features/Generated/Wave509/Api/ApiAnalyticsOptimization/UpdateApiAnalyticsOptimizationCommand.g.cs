namespace AtlasOps.Features.Api.ApiAnalyticsOptimization;

public sealed record UpdateApiAnalyticsOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);