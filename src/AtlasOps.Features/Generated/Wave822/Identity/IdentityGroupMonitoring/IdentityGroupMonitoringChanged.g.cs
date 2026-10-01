namespace AtlasOps.Features.Identity.IdentityGroupMonitoring;

public sealed record IdentityGroupMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);