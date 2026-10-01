namespace AtlasOps.Features.Messaging.MessageQueueMonitoring;

public sealed record UpdateMessageQueueMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);