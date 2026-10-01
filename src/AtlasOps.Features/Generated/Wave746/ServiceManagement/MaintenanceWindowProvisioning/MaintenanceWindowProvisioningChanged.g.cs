namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowProvisioning;

public sealed record MaintenanceWindowProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);