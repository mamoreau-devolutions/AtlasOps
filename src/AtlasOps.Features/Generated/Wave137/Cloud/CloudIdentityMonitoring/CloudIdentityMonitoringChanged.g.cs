namespace AtlasOps.Features.Cloud.CloudIdentityMonitoring;

public sealed record CloudIdentityMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);