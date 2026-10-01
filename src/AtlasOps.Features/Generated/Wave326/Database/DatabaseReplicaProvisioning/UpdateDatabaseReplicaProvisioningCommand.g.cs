namespace AtlasOps.Features.Database.DatabaseReplicaProvisioning;

public sealed record UpdateDatabaseReplicaProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);