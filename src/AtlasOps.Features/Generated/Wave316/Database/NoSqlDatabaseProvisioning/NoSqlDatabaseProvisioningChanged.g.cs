namespace AtlasOps.Features.Database.NoSqlDatabaseProvisioning;

public sealed record NoSqlDatabaseProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);