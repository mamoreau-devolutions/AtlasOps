namespace AtlasOps.Features.Database.DatabaseMaintenanceProvisioning;

public sealed record DatabaseMaintenanceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);