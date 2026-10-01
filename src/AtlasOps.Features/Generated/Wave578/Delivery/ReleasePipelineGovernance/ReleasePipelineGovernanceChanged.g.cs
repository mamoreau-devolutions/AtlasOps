namespace AtlasOps.Features.Delivery.ReleasePipelineGovernance;

public sealed record ReleasePipelineGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);