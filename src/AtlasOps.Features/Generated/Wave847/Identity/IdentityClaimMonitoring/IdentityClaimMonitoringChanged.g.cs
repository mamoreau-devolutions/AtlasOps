namespace AtlasOps.Features.Identity.IdentityClaimMonitoring;

public sealed record IdentityClaimMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);