namespace AtlasOps.Features.ServiceManagement.ServiceDependencyProvisioning;

public sealed record UpdateServiceDependencyProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);