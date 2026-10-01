namespace AtlasOps.Features.Database.DatabaseMaintenanceRecovery;

public sealed record UpdateDatabaseMaintenanceRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);