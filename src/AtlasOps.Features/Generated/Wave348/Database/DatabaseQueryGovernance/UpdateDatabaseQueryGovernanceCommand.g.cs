namespace AtlasOps.Features.Database.DatabaseQueryGovernance;

public sealed record UpdateDatabaseQueryGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);