namespace AtlasOps.Features.Desktop.DesktopLicenseRecovery;

public sealed record DesktopLicenseRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);