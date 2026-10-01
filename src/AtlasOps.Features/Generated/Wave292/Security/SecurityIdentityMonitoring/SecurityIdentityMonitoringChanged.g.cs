namespace AtlasOps.Features.Security.SecurityIdentityMonitoring;

public sealed record SecurityIdentityMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);