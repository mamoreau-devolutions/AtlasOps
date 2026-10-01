namespace AtlasOps.Features.Database.DatabaseMaintenanceRecovery;

public sealed record DatabaseMaintenanceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);