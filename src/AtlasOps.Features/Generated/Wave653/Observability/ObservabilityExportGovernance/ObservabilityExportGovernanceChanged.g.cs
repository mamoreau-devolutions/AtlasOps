namespace AtlasOps.Features.Observability.ObservabilityExportGovernance;

public sealed record ObservabilityExportGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);