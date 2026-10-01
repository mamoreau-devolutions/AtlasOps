namespace AtlasOps.Features.Security.SecurityCertificateRecovery;

public sealed record SecurityCertificateRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);