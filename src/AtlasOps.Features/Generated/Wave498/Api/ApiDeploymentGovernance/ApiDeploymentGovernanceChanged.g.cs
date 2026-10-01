namespace AtlasOps.Features.Api.ApiDeploymentGovernance;

public sealed record ApiDeploymentGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);