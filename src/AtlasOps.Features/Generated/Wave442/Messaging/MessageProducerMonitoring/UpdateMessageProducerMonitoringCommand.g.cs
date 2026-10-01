namespace AtlasOps.Features.Messaging.MessageProducerMonitoring;

public sealed record UpdateMessageProducerMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);