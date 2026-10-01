namespace AtlasOps.Features.Connections.GatewayRouting;

public sealed record GatewayRoutingChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);