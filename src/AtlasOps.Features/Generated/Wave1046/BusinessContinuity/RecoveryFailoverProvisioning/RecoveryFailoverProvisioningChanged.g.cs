namespace AtlasOps.Features.BusinessContinuity.RecoveryFailoverProvisioning;

public sealed record RecoveryFailoverProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);