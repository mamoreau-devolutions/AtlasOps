namespace AtlasOps.Features.Governance.GovernanceAttestation;

public sealed record GovernanceAttestationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);