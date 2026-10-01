namespace AtlasOps.Features.Observability.ObservabilityDashboardGovernance;

public sealed record ObservabilityDashboardGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);