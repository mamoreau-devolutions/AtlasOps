namespace AtlasOps.Features.Architecture.ArchitectureRiskRecovery;

public sealed record ArchitectureRiskRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);