namespace AtlasOps.Features.Data.DataTransformGovernance;

public sealed record DataTransformGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);