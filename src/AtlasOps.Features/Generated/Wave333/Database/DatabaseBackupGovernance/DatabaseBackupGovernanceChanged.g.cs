namespace AtlasOps.Features.Database.DatabaseBackupGovernance;

public sealed record DatabaseBackupGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);