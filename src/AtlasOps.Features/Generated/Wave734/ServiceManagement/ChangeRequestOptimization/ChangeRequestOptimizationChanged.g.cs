namespace AtlasOps.Features.ServiceManagement.ChangeRequestOptimization;

public sealed record ChangeRequestOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);