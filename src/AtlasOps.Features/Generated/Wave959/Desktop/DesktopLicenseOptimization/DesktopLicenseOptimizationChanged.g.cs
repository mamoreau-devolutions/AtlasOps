namespace AtlasOps.Features.Desktop.DesktopLicenseOptimization;

public sealed record DesktopLicenseOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);