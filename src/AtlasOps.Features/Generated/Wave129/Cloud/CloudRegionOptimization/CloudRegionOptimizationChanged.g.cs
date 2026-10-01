namespace AtlasOps.Features.Cloud.CloudRegionOptimization;

public sealed record CloudRegionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);