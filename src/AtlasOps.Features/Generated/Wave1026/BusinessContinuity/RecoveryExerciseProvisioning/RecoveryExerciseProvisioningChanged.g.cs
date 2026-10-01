namespace AtlasOps.Features.BusinessContinuity.RecoveryExerciseProvisioning;

public sealed record RecoveryExerciseProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);