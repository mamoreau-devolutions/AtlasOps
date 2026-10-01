namespace AtlasOps.Features.Compute.ComputeImageGovernance;

public sealed record ComputeImageGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);