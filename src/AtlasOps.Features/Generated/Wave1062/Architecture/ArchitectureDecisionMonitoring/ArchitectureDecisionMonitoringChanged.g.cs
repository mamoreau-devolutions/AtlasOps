namespace AtlasOps.Features.Architecture.ArchitectureDecisionMonitoring;

public sealed record ArchitectureDecisionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);