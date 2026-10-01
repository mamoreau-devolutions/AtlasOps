namespace AtlasOps.Features.Api.ApiGatewayGovernance;

public sealed record ApiGatewayGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);