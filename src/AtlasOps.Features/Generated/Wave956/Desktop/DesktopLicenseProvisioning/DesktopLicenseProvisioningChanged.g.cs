namespace AtlasOps.Features.Desktop.DesktopLicenseProvisioning;

public sealed record DesktopLicenseProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);