namespace AtlasOps.Features.Analytics.QueryWidget;

public sealed record UpdateQueryWidgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);