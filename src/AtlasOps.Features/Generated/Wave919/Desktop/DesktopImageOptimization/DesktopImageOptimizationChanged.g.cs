namespace AtlasOps.Features.Desktop.DesktopImageOptimization;

public sealed record DesktopImageOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);