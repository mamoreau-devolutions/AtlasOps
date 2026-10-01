namespace AtlasOps.Features.Storage.FileShareGovernance;

public sealed record FileShareGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);