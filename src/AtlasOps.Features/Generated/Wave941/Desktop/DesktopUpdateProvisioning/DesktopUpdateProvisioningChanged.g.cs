namespace AtlasOps.Features.Desktop.DesktopUpdateProvisioning;

public sealed record DesktopUpdateProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);