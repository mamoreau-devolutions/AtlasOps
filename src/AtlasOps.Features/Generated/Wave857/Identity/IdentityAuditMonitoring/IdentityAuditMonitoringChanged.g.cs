namespace AtlasOps.Features.Identity.IdentityAuditMonitoring;

public sealed record IdentityAuditMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);