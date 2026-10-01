namespace AtlasOps.Features.Messaging.MessageDeadLetterMonitoring;

public sealed record UpdateMessageDeadLetterMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);