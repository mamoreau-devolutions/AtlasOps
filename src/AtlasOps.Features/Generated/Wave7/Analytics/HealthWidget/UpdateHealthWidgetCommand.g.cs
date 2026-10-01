namespace AtlasOps.Features.Analytics.HealthWidget;

public sealed record UpdateHealthWidgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);