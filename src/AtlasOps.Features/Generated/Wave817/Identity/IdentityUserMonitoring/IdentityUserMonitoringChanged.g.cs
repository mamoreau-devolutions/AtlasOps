namespace AtlasOps.Features.Identity.IdentityUserMonitoring;

public sealed record IdentityUserMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);