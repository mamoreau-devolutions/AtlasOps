namespace AtlasOps.Features.Edge.EdgeTelemetryRecovery;

public sealed record UpdateEdgeTelemetryRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);