namespace AtlasOps.Features.Mobile.MobilePolicyOptimization;

public sealed record MobilePolicyOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);