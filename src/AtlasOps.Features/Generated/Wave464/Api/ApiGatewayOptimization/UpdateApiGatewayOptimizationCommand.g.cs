namespace AtlasOps.Features.Api.ApiGatewayOptimization;

public sealed record UpdateApiGatewayOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);