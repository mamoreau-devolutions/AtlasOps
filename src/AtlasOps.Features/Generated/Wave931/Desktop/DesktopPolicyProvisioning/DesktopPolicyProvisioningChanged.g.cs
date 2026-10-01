namespace AtlasOps.Features.Desktop.DesktopPolicyProvisioning;

public sealed record DesktopPolicyProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);