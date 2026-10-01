namespace AtlasOps.Features.Architecture.ArchitectureStandardMonitoring;

public sealed record ArchitectureStandardMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);