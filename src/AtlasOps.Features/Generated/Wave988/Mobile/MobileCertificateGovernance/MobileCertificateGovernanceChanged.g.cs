namespace AtlasOps.Features.Mobile.MobileCertificateGovernance;

public sealed record MobileCertificateGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);