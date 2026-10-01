namespace AtlasOps.Features.Compute.ComputeLifecycleGovernance;

public sealed record ComputeLifecycleGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);