namespace AtlasOps.Features.Edge.EdgeGatewayGovernance;

public sealed record EdgeGatewayGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);