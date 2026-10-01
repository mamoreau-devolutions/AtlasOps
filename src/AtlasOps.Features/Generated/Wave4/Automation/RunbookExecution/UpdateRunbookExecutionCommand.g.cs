namespace AtlasOps.Features.Automation.RunbookExecution;

public sealed record UpdateRunbookExecutionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);