namespace AtlasOps.Features.FinOps.ResourceCommitmentRecovery;

public sealed record ResourceCommitmentRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);