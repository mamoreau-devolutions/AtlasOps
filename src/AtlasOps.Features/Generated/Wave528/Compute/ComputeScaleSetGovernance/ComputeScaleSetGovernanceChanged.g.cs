namespace AtlasOps.Features.Compute.ComputeScaleSetGovernance;

public sealed record ComputeScaleSetGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);