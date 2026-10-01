namespace AtlasOps.Features.Messaging.MessageTopicMonitoring;

public sealed record UpdateMessageTopicMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);