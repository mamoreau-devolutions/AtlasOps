namespace AtlasOps.Features.Compute.ComputeMetricGovernance;

public sealed record ComputeMetricGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);