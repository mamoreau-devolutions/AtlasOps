namespace AtlasOps.Features.Automation.ScheduledRunbook;

public sealed record UpdateScheduledRunbookCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);