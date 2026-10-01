namespace AtlasOps.Features.Desktop.DesktopPeripheralProvisioning;

public sealed record DesktopPeripheralProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);