namespace AtlasOps.Features.Connections.HttpEndpoint;

public sealed record UpdateHttpEndpointCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);