namespace AtlasOps.Features.Desktop.DesktopPeripheralOptimization;

public sealed record DesktopPeripheralOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);