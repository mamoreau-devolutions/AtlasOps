namespace AtlasOps.Features.ServiceManagement.ServiceCatalogGovernance;

public sealed record ServiceCatalogGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);