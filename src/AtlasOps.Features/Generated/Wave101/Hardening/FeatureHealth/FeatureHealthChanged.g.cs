namespace AtlasOps.Features.Hardening.FeatureHealth;

public sealed record FeatureHealthChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);