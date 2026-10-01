namespace AtlasOps.Features.Edge.EdgeTelemetryProvisioning;

public sealed record UpdateEdgeTelemetryProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);