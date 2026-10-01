namespace AtlasOps.Features.ServiceManagement.ServiceReviewMonitoring;

public sealed record UpdateServiceReviewMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);