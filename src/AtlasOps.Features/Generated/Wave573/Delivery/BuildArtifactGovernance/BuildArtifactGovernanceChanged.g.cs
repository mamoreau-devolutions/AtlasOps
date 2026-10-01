namespace AtlasOps.Features.Delivery.BuildArtifactGovernance;

public sealed record BuildArtifactGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);