namespace AtlasOps.Features.Mobile.MobileUpdateProvisioning;

public sealed record MobileUpdateProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);