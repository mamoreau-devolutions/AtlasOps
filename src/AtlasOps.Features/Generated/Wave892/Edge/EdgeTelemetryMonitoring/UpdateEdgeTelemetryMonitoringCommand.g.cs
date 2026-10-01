namespace AtlasOps.Features.Edge.EdgeTelemetryMonitoring;

public sealed record UpdateEdgeTelemetryMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);