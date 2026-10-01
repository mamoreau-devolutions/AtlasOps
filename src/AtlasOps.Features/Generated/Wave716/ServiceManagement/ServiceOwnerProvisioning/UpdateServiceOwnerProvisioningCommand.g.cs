namespace AtlasOps.Features.ServiceManagement.ServiceOwnerProvisioning;

public sealed record UpdateServiceOwnerProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);