namespace AtlasOps.Features.BusinessContinuity.RecoveryEvidenceProvisioning;

public sealed record RecoveryEvidenceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);