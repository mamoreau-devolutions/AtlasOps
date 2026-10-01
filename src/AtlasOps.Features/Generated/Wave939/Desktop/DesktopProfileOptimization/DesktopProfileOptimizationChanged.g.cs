namespace AtlasOps.Features.Desktop.DesktopProfileOptimization;

public sealed record DesktopProfileOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);