namespace AtlasOps.Features.Automation.ExecutionCheckpoint;

public sealed record UpdateExecutionCheckpointCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);