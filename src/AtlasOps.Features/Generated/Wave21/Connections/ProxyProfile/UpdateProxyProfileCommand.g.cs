namespace AtlasOps.Features.Connections.ProxyProfile;

public sealed record UpdateProxyProfileCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);