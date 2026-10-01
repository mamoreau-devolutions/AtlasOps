namespace AtlasOps.Features.Database.SqlDatabaseProvisioning;

public sealed record SqlDatabaseProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);