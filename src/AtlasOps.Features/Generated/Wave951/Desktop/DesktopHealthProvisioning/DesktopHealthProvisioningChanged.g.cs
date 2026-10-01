namespace AtlasOps.Features.Desktop.DesktopHealthProvisioning;

public sealed record DesktopHealthProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);