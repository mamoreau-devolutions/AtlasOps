namespace AtlasOps.Features.Architecture.ArchitectureDependencyRecovery;

public sealed record ArchitectureDependencyRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);