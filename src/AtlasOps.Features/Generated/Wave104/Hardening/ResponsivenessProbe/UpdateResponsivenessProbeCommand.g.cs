namespace AtlasOps.Features.Hardening.ResponsivenessProbe;

public sealed record UpdateResponsivenessProbeCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);