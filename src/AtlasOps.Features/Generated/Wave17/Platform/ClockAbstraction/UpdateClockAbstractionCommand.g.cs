namespace AtlasOps.Features.Platform.ClockAbstraction;

public sealed record UpdateClockAbstractionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);