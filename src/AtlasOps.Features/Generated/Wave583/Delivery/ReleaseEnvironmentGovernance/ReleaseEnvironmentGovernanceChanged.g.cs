namespace AtlasOps.Features.Delivery.ReleaseEnvironmentGovernance;

public sealed record ReleaseEnvironmentGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);