namespace AtlasOps.Features.Api.ApiContractProvisioning;

public sealed record ApiContractProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);