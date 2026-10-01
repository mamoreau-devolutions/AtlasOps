namespace AtlasOps.Features.Automation.StepRetry;

public sealed record UpdateStepRetryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);