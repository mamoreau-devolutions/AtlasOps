namespace AtlasOps.Features.Automation.ConditionalStep;

public sealed record ConditionalStepChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);