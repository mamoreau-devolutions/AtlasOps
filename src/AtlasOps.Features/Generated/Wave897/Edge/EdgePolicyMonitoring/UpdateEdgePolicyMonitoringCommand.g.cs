namespace AtlasOps.Features.Edge.EdgePolicyMonitoring;

public sealed record UpdateEdgePolicyMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);