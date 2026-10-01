namespace AtlasOps.Features.Messaging.MessageRetentionMonitoring;

public sealed record UpdateMessageRetentionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);