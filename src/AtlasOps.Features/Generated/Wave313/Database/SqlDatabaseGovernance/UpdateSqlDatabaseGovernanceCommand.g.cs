namespace AtlasOps.Features.Database.SqlDatabaseGovernance;

public sealed record UpdateSqlDatabaseGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);