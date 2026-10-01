namespace AtlasOps.Features.Messaging.MessageReplayMonitoring;

public sealed record UpdateMessageReplayMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);