namespace AtlasOps.Features.Desktop.DesktopPoolProvisioning;

public sealed record DesktopPoolProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);