namespace AtlasOps.Features.Architecture.ArchitectureEvidenceGovernance;

public sealed record ArchitectureEvidenceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);