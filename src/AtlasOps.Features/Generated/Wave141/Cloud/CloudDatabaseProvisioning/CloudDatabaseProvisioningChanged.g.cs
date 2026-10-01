namespace AtlasOps.Features.Cloud.CloudDatabaseProvisioning;

public sealed record CloudDatabaseProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);