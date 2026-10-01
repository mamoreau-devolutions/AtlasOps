namespace AtlasOps.Features.Observability.ObservabilityDashboardRecovery;

public sealed record ObservabilityDashboardRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);