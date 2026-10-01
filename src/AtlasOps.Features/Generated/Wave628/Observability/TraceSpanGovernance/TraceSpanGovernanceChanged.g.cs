namespace AtlasOps.Features.Observability.TraceSpanGovernance;

public sealed record TraceSpanGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);