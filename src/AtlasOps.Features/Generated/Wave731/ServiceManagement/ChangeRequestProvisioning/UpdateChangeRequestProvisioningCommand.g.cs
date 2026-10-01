namespace AtlasOps.Features.ServiceManagement.ChangeRequestProvisioning;

public sealed record UpdateChangeRequestProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);