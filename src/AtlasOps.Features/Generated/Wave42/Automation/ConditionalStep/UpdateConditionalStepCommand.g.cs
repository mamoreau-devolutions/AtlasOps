namespace AtlasOps.Features.Automation.ConditionalStep;

public sealed record UpdateConditionalStepCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);