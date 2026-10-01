namespace AtlasOps.Features.Desktop.DesktopUpdateOptimization;

public sealed record DesktopUpdateOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);