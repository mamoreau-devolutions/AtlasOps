namespace AtlasOps.Features.Security.SecurityExceptionProvisioning;

public sealed record SecurityExceptionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);