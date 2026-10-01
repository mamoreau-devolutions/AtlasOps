namespace AtlasOps.Features.ServiceManagement.ServiceRequestProvisioning;

public sealed record ServiceRequestProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);