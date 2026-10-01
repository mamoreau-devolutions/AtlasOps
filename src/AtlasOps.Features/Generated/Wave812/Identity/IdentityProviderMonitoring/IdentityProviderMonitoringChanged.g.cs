namespace AtlasOps.Features.Identity.IdentityProviderMonitoring;

public sealed record IdentityProviderMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);