namespace AtlasOps.Features.Architecture.ArchitectureDependencyMonitoring;

public sealed record ArchitectureDependencyMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);