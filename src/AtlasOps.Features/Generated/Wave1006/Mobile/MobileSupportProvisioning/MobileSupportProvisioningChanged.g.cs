namespace AtlasOps.Features.Mobile.MobileSupportProvisioning;

public sealed record MobileSupportProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);