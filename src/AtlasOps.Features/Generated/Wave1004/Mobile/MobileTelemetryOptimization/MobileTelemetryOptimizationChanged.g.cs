namespace AtlasOps.Features.Mobile.MobileTelemetryOptimization;

public sealed record MobileTelemetryOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);