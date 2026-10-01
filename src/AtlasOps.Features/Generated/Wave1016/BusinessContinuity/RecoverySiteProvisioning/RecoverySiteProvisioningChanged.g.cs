namespace AtlasOps.Features.BusinessContinuity.RecoverySiteProvisioning;

public sealed record RecoverySiteProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);