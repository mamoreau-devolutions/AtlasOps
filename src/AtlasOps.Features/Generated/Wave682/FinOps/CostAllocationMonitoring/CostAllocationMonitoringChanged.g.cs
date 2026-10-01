namespace AtlasOps.Features.FinOps.CostAllocationMonitoring;

public sealed record CostAllocationMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);