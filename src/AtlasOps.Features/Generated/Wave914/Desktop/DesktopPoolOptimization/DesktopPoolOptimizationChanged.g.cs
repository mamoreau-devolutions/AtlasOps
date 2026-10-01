namespace AtlasOps.Features.Desktop.DesktopPoolOptimization;

public sealed record DesktopPoolOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);