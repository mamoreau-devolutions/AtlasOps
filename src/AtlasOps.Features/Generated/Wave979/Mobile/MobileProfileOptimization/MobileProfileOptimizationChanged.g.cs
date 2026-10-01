namespace AtlasOps.Features.Mobile.MobileProfileOptimization;

public sealed record MobileProfileOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);