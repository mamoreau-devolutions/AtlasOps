namespace AtlasOps.Features.Mobile.MobileUpdateOptimization;

public sealed record MobileUpdateOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);