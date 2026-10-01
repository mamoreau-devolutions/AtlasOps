namespace AtlasOps.Features.Api.ApiQuotaProvisioning;

public sealed record ApiQuotaProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);