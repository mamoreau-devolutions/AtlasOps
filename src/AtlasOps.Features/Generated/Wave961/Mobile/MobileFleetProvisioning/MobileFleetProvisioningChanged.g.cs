namespace AtlasOps.Features.Mobile.MobileFleetProvisioning;

public sealed record MobileFleetProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);