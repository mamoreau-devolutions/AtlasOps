namespace AtlasOps.Features.Database.DatabaseReplicaGovernance;

public sealed record UpdateDatabaseReplicaGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);