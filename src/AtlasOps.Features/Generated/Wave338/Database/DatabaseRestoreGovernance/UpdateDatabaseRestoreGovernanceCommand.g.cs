namespace AtlasOps.Features.Database.DatabaseRestoreGovernance;

public sealed record UpdateDatabaseRestoreGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);