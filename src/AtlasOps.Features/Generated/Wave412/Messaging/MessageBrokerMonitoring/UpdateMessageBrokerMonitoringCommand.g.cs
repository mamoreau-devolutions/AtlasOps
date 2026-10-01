namespace AtlasOps.Features.Messaging.MessageBrokerMonitoring;

public sealed record UpdateMessageBrokerMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);