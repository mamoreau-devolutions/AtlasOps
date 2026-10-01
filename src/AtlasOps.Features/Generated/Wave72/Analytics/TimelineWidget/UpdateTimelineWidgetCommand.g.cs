namespace AtlasOps.Features.Analytics.TimelineWidget;

public sealed record UpdateTimelineWidgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);