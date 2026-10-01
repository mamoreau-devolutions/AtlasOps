namespace AtlasOps.Features.Mobile.MobileSupportOptimization;

public sealed record MobileSupportOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);