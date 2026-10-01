namespace AtlasOps.Features.Database.DatabaseMaintenanceOptimization;

public sealed record DatabaseMaintenanceOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);