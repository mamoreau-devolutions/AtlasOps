namespace AtlasOps.Features.Platform.CommandTelemetry;

public sealed record CommandTelemetryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);