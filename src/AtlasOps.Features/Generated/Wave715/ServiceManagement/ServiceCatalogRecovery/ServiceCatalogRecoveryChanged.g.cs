namespace AtlasOps.Features.ServiceManagement.ServiceCatalogRecovery;

public sealed record ServiceCatalogRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);