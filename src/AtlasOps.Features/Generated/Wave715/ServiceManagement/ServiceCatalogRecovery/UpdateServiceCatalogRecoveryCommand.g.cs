namespace AtlasOps.Features.ServiceManagement.ServiceCatalogRecovery;

public sealed record UpdateServiceCatalogRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);