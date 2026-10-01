namespace AtlasOps.Features.Automation.BlueGreenRollout;

public sealed record UpdateBlueGreenRolloutCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);