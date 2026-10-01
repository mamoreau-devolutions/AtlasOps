namespace AtlasOps.Features.Compute.ComputeConsoleOptimization;

public sealed record ComputeConsoleOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);