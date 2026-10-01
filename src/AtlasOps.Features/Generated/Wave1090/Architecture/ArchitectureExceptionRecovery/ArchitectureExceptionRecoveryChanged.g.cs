namespace AtlasOps.Features.Architecture.ArchitectureExceptionRecovery;

public sealed record ArchitectureExceptionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);