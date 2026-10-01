namespace AtlasOps.Features.Hardening.StartupProbe;

public sealed record StartupProbeChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);