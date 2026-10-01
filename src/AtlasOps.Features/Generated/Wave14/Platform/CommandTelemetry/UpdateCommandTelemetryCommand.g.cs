namespace AtlasOps.Features.Platform.CommandTelemetry;

public sealed record UpdateCommandTelemetryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);