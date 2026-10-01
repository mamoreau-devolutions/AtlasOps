namespace AtlasOps.Features.Data.DataSourceProvisioning;

public sealed record DataSourceProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);