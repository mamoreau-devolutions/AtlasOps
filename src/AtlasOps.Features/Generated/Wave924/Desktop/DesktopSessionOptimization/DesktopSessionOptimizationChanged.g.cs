namespace AtlasOps.Features.Desktop.DesktopSessionOptimization;

public sealed record DesktopSessionOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);