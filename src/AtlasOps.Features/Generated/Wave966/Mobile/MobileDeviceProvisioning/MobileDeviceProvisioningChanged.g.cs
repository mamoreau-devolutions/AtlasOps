namespace AtlasOps.Features.Mobile.MobileDeviceProvisioning;

public sealed record MobileDeviceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);