namespace AtlasOps.Features.Editor.ResultProjection;

public sealed record UpdateResultProjectionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);