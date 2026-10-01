namespace AtlasOps.Features.Delivery.ReleaseCalendarOptimization;

public sealed record ReleaseCalendarOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);