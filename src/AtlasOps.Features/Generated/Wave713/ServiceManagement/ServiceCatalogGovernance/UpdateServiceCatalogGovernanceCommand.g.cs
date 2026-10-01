namespace AtlasOps.Features.ServiceManagement.ServiceCatalogGovernance;

public sealed record UpdateServiceCatalogGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);