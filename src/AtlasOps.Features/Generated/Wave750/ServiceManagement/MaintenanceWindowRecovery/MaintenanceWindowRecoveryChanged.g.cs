namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowRecovery;

public sealed record MaintenanceWindowRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);