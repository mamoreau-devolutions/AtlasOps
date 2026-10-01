namespace AtlasOps.Features.Compute.ComputeTemplateGovernance;

public sealed record ComputeTemplateGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);