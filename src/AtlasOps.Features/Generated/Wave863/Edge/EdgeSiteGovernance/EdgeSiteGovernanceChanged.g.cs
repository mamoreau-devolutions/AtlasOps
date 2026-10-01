namespace AtlasOps.Features.Edge.EdgeSiteGovernance;

public sealed record EdgeSiteGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);