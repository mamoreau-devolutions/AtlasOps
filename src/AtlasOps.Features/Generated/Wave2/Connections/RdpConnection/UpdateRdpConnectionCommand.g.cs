namespace AtlasOps.Features.Connections.RdpConnection;

public sealed record UpdateRdpConnectionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);