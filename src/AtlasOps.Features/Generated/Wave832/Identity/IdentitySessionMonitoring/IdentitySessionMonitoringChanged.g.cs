namespace AtlasOps.Features.Identity.IdentitySessionMonitoring;

public sealed record IdentitySessionMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);