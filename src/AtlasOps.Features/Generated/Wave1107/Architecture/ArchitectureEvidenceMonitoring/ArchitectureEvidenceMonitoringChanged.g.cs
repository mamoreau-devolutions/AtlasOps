namespace AtlasOps.Features.Architecture.ArchitectureEvidenceMonitoring;

public sealed record ArchitectureEvidenceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);