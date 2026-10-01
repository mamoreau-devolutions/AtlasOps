namespace AtlasOps.Features.Security.SecurityCertificateGovernance;

public sealed record SecurityCertificateGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);