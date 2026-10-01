namespace AtlasOps.Features.Storage.StorageEncryptionMonitoring;

public sealed record UpdateStorageEncryptionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);