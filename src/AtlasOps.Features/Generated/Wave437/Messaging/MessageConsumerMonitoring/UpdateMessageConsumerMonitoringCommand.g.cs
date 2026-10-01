namespace AtlasOps.Features.Messaging.MessageConsumerMonitoring;

public sealed record UpdateMessageConsumerMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);