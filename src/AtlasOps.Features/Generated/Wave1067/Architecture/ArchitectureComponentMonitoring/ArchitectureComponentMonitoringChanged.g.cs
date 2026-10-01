namespace AtlasOps.Features.Architecture.ArchitectureComponentMonitoring;

public sealed record ArchitectureComponentMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);