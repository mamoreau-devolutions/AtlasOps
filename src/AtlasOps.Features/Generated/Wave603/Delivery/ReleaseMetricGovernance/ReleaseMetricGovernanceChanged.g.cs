namespace AtlasOps.Features.Delivery.ReleaseMetricGovernance;

public sealed record ReleaseMetricGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);