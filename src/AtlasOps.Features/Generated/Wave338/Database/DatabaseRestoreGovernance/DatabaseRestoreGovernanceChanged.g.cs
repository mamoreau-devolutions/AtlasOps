namespace AtlasOps.Features.Database.DatabaseRestoreGovernance;

public sealed record DatabaseRestoreGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);