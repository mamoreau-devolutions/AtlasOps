namespace AtlasOps.Features.Database.DatabaseIndexMonitoring;

public sealed record DatabaseIndexMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);