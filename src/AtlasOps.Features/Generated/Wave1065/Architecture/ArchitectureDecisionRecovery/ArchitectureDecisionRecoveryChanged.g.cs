namespace AtlasOps.Features.Architecture.ArchitectureDecisionRecovery;

public sealed record ArchitectureDecisionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);