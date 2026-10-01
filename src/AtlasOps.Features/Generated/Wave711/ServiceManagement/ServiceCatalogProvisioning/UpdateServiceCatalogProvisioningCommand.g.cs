namespace AtlasOps.Features.ServiceManagement.ServiceCatalogProvisioning;

public sealed record UpdateServiceCatalogProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);