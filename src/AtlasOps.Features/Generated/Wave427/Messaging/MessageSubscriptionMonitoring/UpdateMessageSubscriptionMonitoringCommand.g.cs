namespace AtlasOps.Features.Messaging.MessageSubscriptionMonitoring;

public sealed record UpdateMessageSubscriptionMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);