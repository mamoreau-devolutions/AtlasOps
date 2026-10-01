namespace AtlasOps.Features.Edge.EdgeTelemetryGovernance;

public sealed record UpdateEdgeTelemetryGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);