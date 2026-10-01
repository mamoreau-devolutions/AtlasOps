namespace AtlasOps.Features.ServiceManagement.MaintenanceWindowOptimization;

public sealed record MaintenanceWindowOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);