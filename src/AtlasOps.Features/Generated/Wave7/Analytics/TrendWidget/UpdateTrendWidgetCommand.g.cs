namespace AtlasOps.Features.Analytics.TrendWidget;

public sealed record UpdateTrendWidgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);