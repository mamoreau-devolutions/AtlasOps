namespace AtlasOps.Features.Incidents.RotationHandoff;

public sealed record UpdateRotationHandoffCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);