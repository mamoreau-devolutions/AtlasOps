namespace AtlasOps.Features.Desktop.DesktopHealthOptimization;

public sealed record DesktopHealthOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);