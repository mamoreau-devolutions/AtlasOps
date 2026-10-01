namespace AtlasOps.Features.Mobile.MobileFleetOptimization;

public sealed record MobileFleetOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);