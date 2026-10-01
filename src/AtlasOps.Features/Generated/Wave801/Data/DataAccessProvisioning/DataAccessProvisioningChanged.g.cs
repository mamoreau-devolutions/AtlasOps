namespace AtlasOps.Features.Data.DataAccessProvisioning;

public sealed record DataAccessProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);