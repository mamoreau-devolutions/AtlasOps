namespace AtlasOps.Features.Edge.EdgeDeviceOptimization;

public sealed record EdgeDeviceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);