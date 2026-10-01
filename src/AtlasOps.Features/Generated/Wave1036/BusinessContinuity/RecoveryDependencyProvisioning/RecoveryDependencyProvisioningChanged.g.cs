namespace AtlasOps.Features.BusinessContinuity.RecoveryDependencyProvisioning;

public sealed record RecoveryDependencyProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);