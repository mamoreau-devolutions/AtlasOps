namespace AtlasOps.Features.ServiceManagement.ServiceCatalogOptimization;

public sealed record UpdateServiceCatalogOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);