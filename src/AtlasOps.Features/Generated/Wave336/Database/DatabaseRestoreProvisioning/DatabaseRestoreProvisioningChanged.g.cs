namespace AtlasOps.Features.Database.DatabaseRestoreProvisioning;

public sealed record DatabaseRestoreProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);