namespace AtlasOps.Features.Database.DatabaseIndexGovernance;

public sealed record UpdateDatabaseIndexGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);