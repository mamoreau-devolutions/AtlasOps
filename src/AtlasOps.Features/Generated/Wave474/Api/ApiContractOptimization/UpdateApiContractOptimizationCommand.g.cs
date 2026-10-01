namespace AtlasOps.Features.Api.ApiContractOptimization;

public sealed record UpdateApiContractOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);