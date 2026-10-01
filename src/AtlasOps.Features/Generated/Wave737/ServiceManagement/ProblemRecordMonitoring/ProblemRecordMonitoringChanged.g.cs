namespace AtlasOps.Features.ServiceManagement.ProblemRecordMonitoring;

public sealed record ProblemRecordMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);