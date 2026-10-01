namespace AtlasOps.Features.Data.DataRetentionGovernance;

public sealed record DataRetentionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);