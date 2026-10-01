namespace AtlasOps.Features.Desktop.DesktopImageProvisioning;

public sealed record DesktopImageProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);