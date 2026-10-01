namespace AtlasOps.Features.Desktop.DesktopApplicationProvisioning;

public sealed record DesktopApplicationProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);