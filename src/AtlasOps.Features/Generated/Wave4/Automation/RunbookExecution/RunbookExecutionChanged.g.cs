namespace AtlasOps.Features.Automation.RunbookExecution;

public sealed record RunbookExecutionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);