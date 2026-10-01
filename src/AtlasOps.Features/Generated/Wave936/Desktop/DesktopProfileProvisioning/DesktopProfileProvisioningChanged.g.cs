namespace AtlasOps.Features.Desktop.DesktopProfileProvisioning;

public sealed record DesktopProfileProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);