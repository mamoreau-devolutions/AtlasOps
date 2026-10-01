namespace AtlasOps.Features.Identity.IdentityFactorMonitoring;

public sealed record IdentityFactorMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);