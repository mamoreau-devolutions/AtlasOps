namespace AtlasOps.Features.Database.NoSqlDatabaseGovernance;

public sealed record UpdateNoSqlDatabaseGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);