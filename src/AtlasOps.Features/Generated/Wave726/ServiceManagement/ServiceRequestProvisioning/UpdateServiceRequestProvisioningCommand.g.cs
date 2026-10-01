namespace AtlasOps.Features.ServiceManagement.ServiceRequestProvisioning;

public sealed record UpdateServiceRequestProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);