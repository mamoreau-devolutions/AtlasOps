namespace AtlasOps.Features.Analytics.MarkdownWidget;

public sealed record UpdateMarkdownWidgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);