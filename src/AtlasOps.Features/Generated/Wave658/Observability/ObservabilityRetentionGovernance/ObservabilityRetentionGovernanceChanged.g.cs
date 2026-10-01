namespace AtlasOps.Features.Observability.ObservabilityRetentionGovernance;

public sealed record ObservabilityRetentionGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);