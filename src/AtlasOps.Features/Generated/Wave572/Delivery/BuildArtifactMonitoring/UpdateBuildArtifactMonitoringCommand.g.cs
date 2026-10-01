namespace AtlasOps.Features.Delivery.BuildArtifactMonitoring;

public sealed record UpdateBuildArtifactMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);