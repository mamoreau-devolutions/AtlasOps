namespace AtlasOps.Features.Database.DatabaseReplicaRecovery;

public sealed record UpdateDatabaseReplicaRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);