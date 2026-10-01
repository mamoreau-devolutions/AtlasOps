namespace AtlasOps.Features.Edge.EdgeGatewayRecovery;

public sealed record EdgeGatewayRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);