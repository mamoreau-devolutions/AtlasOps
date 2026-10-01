namespace AtlasOps.Features.Connections.LocalProcess;

public sealed record UpdateLocalProcessCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);