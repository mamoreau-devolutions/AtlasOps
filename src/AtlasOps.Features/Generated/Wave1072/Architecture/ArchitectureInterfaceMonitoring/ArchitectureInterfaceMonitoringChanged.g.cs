namespace AtlasOps.Features.Architecture.ArchitectureInterfaceMonitoring;

public sealed record ArchitectureInterfaceMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);