namespace AtlasOps.Features.Api.ApiClientProvisioning;

public sealed record ApiClientProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);