namespace AtlasOps.Features.Automation.ChangeWindow;

public sealed record UpdateChangeWindowCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);