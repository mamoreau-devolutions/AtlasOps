namespace AtlasOps.Features.Connections.PortForwarding;

public sealed record UpdatePortForwardingCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);