namespace AtlasOps.Features.Database.DatabaseReplicaGovernance;

public sealed record DatabaseReplicaGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);