namespace AtlasOps.Features.Data.DataContractProvisioning;

public sealed record DataContractProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);