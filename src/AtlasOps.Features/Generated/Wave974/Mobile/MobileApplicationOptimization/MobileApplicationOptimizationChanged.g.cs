namespace AtlasOps.Features.Mobile.MobileApplicationOptimization;

public sealed record MobileApplicationOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);