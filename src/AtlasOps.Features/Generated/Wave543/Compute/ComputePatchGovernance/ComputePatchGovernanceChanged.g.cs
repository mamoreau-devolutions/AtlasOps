namespace AtlasOps.Features.Compute.ComputePatchGovernance;

public sealed record ComputePatchGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);