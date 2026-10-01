namespace AtlasOps.Features.Connections.GatewayRouting;

public sealed record UpdateGatewayRoutingCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);