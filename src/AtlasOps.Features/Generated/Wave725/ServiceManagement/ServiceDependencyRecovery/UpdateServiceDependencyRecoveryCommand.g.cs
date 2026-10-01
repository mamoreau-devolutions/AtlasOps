namespace AtlasOps.Features.ServiceManagement.ServiceDependencyRecovery;

public sealed record UpdateServiceDependencyRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);