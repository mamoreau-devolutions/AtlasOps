namespace AtlasOps.Features.Analytics.DashboardParameter;

public sealed record UpdateDashboardParameterCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);