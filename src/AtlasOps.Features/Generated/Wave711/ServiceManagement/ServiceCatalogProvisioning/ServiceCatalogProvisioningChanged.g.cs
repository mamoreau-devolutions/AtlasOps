namespace AtlasOps.Features.ServiceManagement.ServiceCatalogProvisioning;

public sealed record ServiceCatalogProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);