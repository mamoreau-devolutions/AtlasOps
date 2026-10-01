namespace AtlasOps.Features.Mobile.MobileDeviceOptimization;

public sealed record MobileDeviceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);