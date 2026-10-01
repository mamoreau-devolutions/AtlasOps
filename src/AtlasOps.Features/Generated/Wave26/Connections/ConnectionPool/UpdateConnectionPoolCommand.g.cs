namespace AtlasOps.Features.Connections.ConnectionPool;

public sealed record UpdateConnectionPoolCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);