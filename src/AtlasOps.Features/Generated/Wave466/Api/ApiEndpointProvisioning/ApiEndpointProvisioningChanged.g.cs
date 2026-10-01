namespace AtlasOps.Features.Api.ApiEndpointProvisioning;

public sealed record ApiEndpointProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);