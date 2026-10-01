namespace AtlasOps.Features.Data.DataPipelineGovernance;

public sealed record DataPipelineGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);