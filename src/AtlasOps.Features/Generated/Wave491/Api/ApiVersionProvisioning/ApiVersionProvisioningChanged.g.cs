namespace AtlasOps.Features.Api.ApiVersionProvisioning;

public sealed record ApiVersionProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);