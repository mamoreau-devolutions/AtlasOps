namespace AtlasOps.Features.Data.DataQualityGovernance;

public sealed record DataQualityGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);