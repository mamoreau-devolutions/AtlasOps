namespace AtlasOps.Features.Analytics.NumberWidget;

public sealed record UpdateNumberWidgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);