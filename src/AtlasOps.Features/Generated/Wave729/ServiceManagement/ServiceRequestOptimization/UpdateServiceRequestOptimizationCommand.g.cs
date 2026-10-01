namespace AtlasOps.Features.ServiceManagement.ServiceRequestOptimization;

public sealed record UpdateServiceRequestOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);