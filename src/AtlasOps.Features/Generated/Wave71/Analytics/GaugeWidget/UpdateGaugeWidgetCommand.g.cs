namespace AtlasOps.Features.Analytics.GaugeWidget;

public sealed record UpdateGaugeWidgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);