namespace AtlasOps.Features.Edge.EdgeTelemetryOptimization;

public sealed record EdgeTelemetryOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);