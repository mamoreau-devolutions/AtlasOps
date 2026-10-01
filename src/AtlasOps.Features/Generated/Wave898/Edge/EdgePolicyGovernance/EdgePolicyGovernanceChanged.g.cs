namespace AtlasOps.Features.Edge.EdgePolicyGovernance;

public sealed record EdgePolicyGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);