namespace AtlasOps.Features.Observability.ObservabilitySloGovernance;

public sealed record ObservabilitySloGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);