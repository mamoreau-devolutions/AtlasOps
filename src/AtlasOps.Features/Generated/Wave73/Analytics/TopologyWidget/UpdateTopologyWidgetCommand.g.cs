namespace AtlasOps.Features.Analytics.TopologyWidget;

public sealed record UpdateTopologyWidgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);