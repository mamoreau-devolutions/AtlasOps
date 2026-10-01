namespace AtlasOps.Features.Database.DatabaseBackupGovernance;

public sealed record UpdateDatabaseBackupGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);