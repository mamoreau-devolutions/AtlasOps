namespace AtlasOps.Features.Architecture.ArchitectureReviewGovernance;

public sealed record ArchitectureReviewGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);