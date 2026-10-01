namespace AtlasOps.Features.Hardening.CrashMarker;

public sealed record UpdateCrashMarkerCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);