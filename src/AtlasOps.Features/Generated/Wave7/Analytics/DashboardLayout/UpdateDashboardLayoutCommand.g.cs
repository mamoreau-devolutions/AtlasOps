namespace AtlasOps.Features.Analytics.DashboardLayout;

public sealed record UpdateDashboardLayoutCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);