namespace AtlasOps.Features.Architecture.ArchitectureReviewMonitoring;

public sealed record ArchitectureReviewMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);