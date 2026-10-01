namespace AtlasOps.Features.Desktop.DesktopSessionProvisioning;

public sealed record DesktopSessionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);