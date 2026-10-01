namespace AtlasOps.Features.Messaging.MessageSchemaMonitoring;

public sealed record UpdateMessageSchemaMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);