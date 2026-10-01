namespace AtlasOps.Features.Edge.EdgePolicyMonitoring;

public sealed record EdgePolicyMonitoringChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);