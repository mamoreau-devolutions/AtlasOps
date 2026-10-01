namespace AtlasOps.Features.Edge.EdgeDeploymentGovernance;

public sealed record EdgeDeploymentGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);