namespace AtlasOps.Features.Identity.IdentityLifecycleMonitoring;

public sealed record IdentityLifecycleMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);