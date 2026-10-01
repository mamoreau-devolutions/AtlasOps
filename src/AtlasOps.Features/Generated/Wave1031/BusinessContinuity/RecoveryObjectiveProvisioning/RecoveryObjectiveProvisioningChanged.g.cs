namespace AtlasOps.Features.BusinessContinuity.RecoveryObjectiveProvisioning;

public sealed record RecoveryObjectiveProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);