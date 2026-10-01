namespace AtlasOps.Features.Identity.IdentityApplicationMonitoring;

public sealed record IdentityApplicationMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);