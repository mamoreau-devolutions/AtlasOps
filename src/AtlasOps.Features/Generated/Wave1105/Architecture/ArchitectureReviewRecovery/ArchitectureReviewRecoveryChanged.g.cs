namespace AtlasOps.Features.Architecture.ArchitectureReviewRecovery;

public sealed record ArchitectureReviewRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);