namespace AtlasOps.Features.Incidents.RemediationTracking;

public sealed record UpdateRemediationTrackingCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);