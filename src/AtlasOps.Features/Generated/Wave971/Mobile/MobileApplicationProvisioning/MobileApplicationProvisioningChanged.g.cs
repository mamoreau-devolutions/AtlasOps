namespace AtlasOps.Features.Mobile.MobileApplicationProvisioning;

public sealed record MobileApplicationProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);