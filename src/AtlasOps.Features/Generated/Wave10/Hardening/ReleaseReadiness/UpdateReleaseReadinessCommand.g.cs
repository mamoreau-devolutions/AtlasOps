namespace AtlasOps.Features.Hardening.ReleaseReadiness;

public sealed record UpdateReleaseReadinessCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);