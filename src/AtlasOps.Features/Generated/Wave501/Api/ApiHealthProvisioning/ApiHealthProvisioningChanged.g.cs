namespace AtlasOps.Features.Api.ApiHealthProvisioning;

public sealed record ApiHealthProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);