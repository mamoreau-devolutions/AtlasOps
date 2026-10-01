namespace AtlasOps.Features.Compute.ComputeScheduleGovernance;

public sealed record ComputeScheduleGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);