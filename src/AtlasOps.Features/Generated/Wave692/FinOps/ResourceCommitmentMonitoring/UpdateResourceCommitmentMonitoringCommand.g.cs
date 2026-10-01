namespace AtlasOps.Features.FinOps.ResourceCommitmentMonitoring;

public sealed record UpdateResourceCommitmentMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);