namespace AtlasOps.Features.ServiceManagement.ServiceDependencyOptimization;

public sealed record UpdateServiceDependencyOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);