namespace AtlasOps.Features.Api.ApiGatewayRecovery;

public sealed record ApiGatewayRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);