namespace AtlasOps.Features.Mobile.MobileTelemetryRecovery;

public sealed record UpdateMobileTelemetryRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);