namespace AtlasOps.Features.Architecture.ArchitectureStandardRecovery;

public sealed record ArchitectureStandardRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);