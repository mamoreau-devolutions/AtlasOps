namespace AtlasOps.Features.Hardening.DependencyProbe;

public sealed record DependencyProbeChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);