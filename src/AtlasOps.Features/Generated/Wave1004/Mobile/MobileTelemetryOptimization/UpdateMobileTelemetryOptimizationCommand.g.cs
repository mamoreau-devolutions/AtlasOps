namespace AtlasOps.Features.Mobile.MobileTelemetryOptimization;

public sealed record UpdateMobileTelemetryOptimizationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);