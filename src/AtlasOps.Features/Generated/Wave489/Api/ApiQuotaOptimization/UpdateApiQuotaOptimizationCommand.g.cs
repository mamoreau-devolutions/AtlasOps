namespace AtlasOps.Features.Api.ApiQuotaOptimization;

public sealed record UpdateApiQuotaOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);