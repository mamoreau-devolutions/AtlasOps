namespace AtlasOps.Features.Platform.ClockAbstraction;

public sealed record ClockAbstractionChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);