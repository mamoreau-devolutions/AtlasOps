namespace AtlasOps.Features.Observability.TraceSourceGovernance;

public sealed record TraceSourceGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);