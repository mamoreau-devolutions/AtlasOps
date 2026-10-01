namespace AtlasOps.Features.Cloud.CloudDatabaseRecovery;

public sealed record UpdateCloudDatabaseRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);