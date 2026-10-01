namespace AtlasOps.Features.Data.DataDatasetProvisioning;

public sealed record DataDatasetProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);