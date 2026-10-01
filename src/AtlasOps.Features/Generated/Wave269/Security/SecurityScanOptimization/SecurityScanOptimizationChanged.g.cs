namespace AtlasOps.Features.Security.SecurityScanOptimization;

public sealed record SecurityScanOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);