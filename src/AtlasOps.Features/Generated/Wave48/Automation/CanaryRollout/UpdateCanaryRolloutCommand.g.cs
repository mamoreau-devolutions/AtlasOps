namespace AtlasOps.Features.Automation.CanaryRollout;

public sealed record UpdateCanaryRolloutCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);