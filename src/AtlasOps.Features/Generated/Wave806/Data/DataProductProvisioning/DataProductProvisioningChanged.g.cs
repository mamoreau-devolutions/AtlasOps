namespace AtlasOps.Features.Data.DataProductProvisioning;

public sealed record DataProductProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);