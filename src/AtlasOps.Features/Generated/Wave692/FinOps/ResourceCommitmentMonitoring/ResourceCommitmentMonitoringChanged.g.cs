namespace AtlasOps.Features.FinOps.ResourceCommitmentMonitoring;

public sealed record ResourceCommitmentMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);