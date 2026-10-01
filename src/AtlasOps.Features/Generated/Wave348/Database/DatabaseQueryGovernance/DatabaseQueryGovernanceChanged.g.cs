namespace AtlasOps.Features.Database.DatabaseQueryGovernance;

public sealed record DatabaseQueryGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);