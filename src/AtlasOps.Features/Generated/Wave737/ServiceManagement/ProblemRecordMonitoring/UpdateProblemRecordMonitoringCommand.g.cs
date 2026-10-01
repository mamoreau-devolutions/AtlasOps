namespace AtlasOps.Features.ServiceManagement.ProblemRecordMonitoring;

public sealed record UpdateProblemRecordMonitoringCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);