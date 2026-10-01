namespace AtlasOps.Features.Architecture.ArchitectureInterfaceRecovery;

public sealed record ArchitectureInterfaceRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);