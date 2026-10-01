namespace AtlasOps.Features.ServiceManagement.ChangeRequestOptimization;

public sealed record UpdateChangeRequestOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);