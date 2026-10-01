namespace AtlasOps.Features.ServiceManagement.ServiceOwnerOptimization;

public sealed record UpdateServiceOwnerOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);