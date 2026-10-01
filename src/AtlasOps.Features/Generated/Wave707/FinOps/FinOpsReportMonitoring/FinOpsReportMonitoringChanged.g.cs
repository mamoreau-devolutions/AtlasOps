namespace AtlasOps.Features.FinOps.FinOpsReportMonitoring;

public sealed record FinOpsReportMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);