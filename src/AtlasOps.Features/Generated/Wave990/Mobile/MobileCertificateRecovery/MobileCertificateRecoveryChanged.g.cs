namespace AtlasOps.Features.Mobile.MobileCertificateRecovery;

public sealed record MobileCertificateRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);