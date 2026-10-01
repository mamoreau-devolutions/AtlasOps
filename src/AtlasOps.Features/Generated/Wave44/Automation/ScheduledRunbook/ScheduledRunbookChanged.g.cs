namespace AtlasOps.Features.Automation.ScheduledRunbook;

public sealed record ScheduledRunbookChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);