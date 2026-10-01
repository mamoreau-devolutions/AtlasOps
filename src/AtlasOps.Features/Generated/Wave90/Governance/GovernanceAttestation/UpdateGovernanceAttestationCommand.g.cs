namespace AtlasOps.Features.Governance.GovernanceAttestation;

public sealed record UpdateGovernanceAttestationCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);