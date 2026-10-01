namespace AtlasOps.Features.Edge.EdgeTelemetryRecovery;

public sealed record EdgeTelemetryRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);