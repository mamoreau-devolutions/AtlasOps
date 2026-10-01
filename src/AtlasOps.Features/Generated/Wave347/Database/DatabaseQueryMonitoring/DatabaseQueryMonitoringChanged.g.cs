namespace AtlasOps.Features.Database.DatabaseQueryMonitoring;

public sealed record DatabaseQueryMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);