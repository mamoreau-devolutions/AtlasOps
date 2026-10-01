namespace AtlasOps.Features.Mobile.MobilePolicyProvisioning;

public sealed record MobilePolicyProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);