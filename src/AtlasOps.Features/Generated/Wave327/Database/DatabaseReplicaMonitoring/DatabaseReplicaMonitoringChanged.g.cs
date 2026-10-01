namespace AtlasOps.Features.Database.DatabaseReplicaMonitoring;

public sealed record DatabaseReplicaMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);