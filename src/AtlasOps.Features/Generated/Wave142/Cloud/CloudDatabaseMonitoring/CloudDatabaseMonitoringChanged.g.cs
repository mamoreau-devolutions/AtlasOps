namespace AtlasOps.Features.Cloud.CloudDatabaseMonitoring;

public sealed record CloudDatabaseMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);