namespace AtlasOps.Features.Database.DatabaseReplicaRecovery;

public sealed record DatabaseReplicaRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);