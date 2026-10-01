namespace AtlasOps.Features.Hardening.ReleaseReadiness;

public sealed record ReleaseReadinessChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);