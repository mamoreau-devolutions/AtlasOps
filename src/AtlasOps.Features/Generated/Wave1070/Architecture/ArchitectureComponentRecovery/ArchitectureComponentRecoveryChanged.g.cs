namespace AtlasOps.Features.Architecture.ArchitectureComponentRecovery;

public sealed record ArchitectureComponentRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);