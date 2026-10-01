namespace AtlasOps.Features.Edge.EdgeTelemetryGovernance;

public sealed record EdgeTelemetryGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);