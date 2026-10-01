namespace AtlasOps.Features.Database.DatabaseSchemaGovernance;

public sealed record UpdateDatabaseSchemaGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);