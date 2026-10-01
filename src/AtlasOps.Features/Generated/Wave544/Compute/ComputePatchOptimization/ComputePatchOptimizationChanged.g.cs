namespace AtlasOps.Features.Compute.ComputePatchOptimization;

public sealed record ComputePatchOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);