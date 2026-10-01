namespace AtlasOps.Features.ServiceManagement.ServiceDependencyOptimization;

public sealed record ServiceDependencyOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);