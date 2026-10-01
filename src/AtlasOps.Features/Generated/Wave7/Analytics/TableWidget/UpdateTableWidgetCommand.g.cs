namespace AtlasOps.Features.Analytics.TableWidget;

public sealed record UpdateTableWidgetCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);