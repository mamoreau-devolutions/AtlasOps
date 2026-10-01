namespace AtlasOps.Features.Api.ApiTokenProvisioning;

public sealed record ApiTokenProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);