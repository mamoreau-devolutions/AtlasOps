namespace AtlasOps.Features.Architecture.ArchitectureEvidenceRecovery;

public sealed record ArchitectureEvidenceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);