namespace AtlasOps.Features.Data.DataLineageProvisioning;

public sealed record DataLineageProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);