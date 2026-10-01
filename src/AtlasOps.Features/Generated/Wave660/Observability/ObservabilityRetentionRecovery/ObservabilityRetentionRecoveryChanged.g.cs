namespace AtlasOps.Features.Observability.ObservabilityRetentionRecovery;

public sealed record ObservabilityRetentionRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);