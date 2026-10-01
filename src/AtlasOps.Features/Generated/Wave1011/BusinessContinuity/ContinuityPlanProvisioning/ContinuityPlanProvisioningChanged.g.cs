namespace AtlasOps.Features.BusinessContinuity.ContinuityPlanProvisioning;

public sealed record ContinuityPlanProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);