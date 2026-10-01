namespace AtlasOps.Features.Compute.ComputeConsoleGovernance;

public sealed record ComputeConsoleGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);