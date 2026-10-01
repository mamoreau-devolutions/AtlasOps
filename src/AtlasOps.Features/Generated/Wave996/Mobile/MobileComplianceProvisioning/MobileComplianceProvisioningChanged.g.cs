namespace AtlasOps.Features.Mobile.MobileComplianceProvisioning;

public sealed record MobileComplianceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);