namespace AtlasOps.Features.Delivery.BuildPipelineGovernance;

public sealed record BuildPipelineGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);