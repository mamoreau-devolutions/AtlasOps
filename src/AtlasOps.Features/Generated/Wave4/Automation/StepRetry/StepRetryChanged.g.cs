namespace AtlasOps.Features.Automation.StepRetry;

public sealed record StepRetryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);