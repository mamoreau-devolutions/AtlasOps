namespace AtlasOps.Features.ServiceManagement.ServiceOwnerOptimization;

public sealed record ServiceOwnerOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);