namespace AtlasOps.Features.Security.SecurityCertificateProvisioning;

public sealed record SecurityCertificateProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);