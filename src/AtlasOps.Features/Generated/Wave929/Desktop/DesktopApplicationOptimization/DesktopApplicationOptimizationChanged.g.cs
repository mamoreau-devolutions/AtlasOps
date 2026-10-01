namespace AtlasOps.Features.Desktop.DesktopApplicationOptimization;

public sealed record DesktopApplicationOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);