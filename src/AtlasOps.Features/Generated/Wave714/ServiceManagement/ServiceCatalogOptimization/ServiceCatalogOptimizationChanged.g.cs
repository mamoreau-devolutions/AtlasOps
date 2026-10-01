namespace AtlasOps.Features.ServiceManagement.ServiceCatalogOptimization;

public sealed record ServiceCatalogOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);