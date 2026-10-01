namespace AtlasOps.Features.Hardening.DependencyProbe;

public sealed record UpdateDependencyProbeCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);