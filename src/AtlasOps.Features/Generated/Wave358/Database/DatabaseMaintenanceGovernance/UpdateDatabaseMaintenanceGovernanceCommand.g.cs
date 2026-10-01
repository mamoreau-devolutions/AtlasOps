namespace AtlasOps.Features.Database.DatabaseMaintenanceGovernance;

public sealed record UpdateDatabaseMaintenanceGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);