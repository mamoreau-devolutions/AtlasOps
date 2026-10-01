namespace AtlasOps.Features.Observability.ObservabilitySloOptimization;

public sealed record ObservabilitySloOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);