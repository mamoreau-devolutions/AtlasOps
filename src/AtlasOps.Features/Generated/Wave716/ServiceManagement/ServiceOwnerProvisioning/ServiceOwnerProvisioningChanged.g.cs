namespace AtlasOps.Features.ServiceManagement.ServiceOwnerProvisioning;

public sealed record ServiceOwnerProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);