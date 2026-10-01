namespace AtlasOps.Features.BusinessContinuity.RecoveryRunbookProvisioning;

public sealed record RecoveryRunbookProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);