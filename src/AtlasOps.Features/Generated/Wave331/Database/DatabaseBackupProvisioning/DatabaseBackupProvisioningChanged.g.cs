namespace AtlasOps.Features.Database.DatabaseBackupProvisioning;

public sealed record DatabaseBackupProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);