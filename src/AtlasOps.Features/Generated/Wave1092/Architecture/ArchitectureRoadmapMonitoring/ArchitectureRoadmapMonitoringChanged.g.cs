namespace AtlasOps.Features.Architecture.ArchitectureRoadmapMonitoring;

public sealed record ArchitectureRoadmapMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);