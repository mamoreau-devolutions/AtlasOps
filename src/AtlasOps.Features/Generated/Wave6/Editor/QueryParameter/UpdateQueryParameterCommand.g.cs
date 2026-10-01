namespace AtlasOps.Features.Editor.QueryParameter;

public sealed record UpdateQueryParameterCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);