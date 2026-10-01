namespace AtlasOps.Features.Mobile.MobileCertificateProvisioning;

public sealed record MobileCertificateProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);