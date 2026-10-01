namespace AtlasOps.Features.Storage.FileShareMonitoring;

public sealed record UpdateFileShareMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);