namespace AtlasOps.Features.Delivery.SourceRepositoryGovernance;

public sealed record SourceRepositoryGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);