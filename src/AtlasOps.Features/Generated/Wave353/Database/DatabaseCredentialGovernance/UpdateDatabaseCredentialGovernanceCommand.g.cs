namespace AtlasOps.Features.Database.DatabaseCredentialGovernance;

public sealed record UpdateDatabaseCredentialGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);