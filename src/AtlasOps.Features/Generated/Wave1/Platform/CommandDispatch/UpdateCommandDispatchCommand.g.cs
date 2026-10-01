namespace AtlasOps.Features.Platform.CommandDispatch;

public sealed record UpdateCommandDispatchCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);