namespace AtlasOps.Features.Architecture.ArchitectureExceptionMonitoring;

public sealed record ArchitectureExceptionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);