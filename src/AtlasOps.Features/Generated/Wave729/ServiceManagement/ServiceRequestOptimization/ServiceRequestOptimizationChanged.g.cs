namespace AtlasOps.Features.ServiceManagement.ServiceRequestOptimization;

public sealed record ServiceRequestOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);