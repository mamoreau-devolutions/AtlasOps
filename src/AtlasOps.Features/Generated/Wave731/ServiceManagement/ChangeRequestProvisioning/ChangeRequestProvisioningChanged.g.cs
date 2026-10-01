namespace AtlasOps.Features.ServiceManagement.ChangeRequestProvisioning;

public sealed record ChangeRequestProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);