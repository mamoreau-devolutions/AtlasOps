namespace AtlasOps.Features.Mobile.MobileProfileProvisioning;

public sealed record MobileProfileProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);