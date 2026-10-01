namespace AtlasOps.Features.Observability.ObservabilityRetentionOptimization;

public sealed record ObservabilityRetentionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);