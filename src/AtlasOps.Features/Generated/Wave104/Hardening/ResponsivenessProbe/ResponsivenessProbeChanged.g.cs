namespace AtlasOps.Features.Hardening.ResponsivenessProbe;

public sealed record ResponsivenessProbeChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);