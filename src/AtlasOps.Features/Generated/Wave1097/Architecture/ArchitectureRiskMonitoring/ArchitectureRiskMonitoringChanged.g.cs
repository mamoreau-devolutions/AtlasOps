namespace AtlasOps.Features.Architecture.ArchitectureRiskMonitoring;

public sealed record ArchitectureRiskMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);