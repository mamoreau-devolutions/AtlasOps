namespace AtlasOps.Features.Cloud.GcpProjectOptimization;

public sealed record GcpProjectOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);