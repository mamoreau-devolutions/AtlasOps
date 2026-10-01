namespace AtlasOps.Features.Mobile.MobileTelemetryGovernance;

public sealed record UpdateMobileTelemetryGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);