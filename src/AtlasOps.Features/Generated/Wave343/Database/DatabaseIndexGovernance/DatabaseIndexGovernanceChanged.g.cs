namespace AtlasOps.Features.Database.DatabaseIndexGovernance;

public sealed record DatabaseIndexGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);