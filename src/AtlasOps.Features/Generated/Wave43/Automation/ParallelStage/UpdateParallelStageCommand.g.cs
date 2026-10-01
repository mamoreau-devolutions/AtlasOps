namespace AtlasOps.Features.Automation.ParallelStage;

public sealed record UpdateParallelStageCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);