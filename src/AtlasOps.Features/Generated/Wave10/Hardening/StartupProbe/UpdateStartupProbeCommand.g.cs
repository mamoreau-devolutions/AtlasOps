namespace AtlasOps.Features.Hardening.StartupProbe;

public sealed record UpdateStartupProbeCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);