namespace AtlasOps.Features.Database.DatabaseReplicaProvisioning;

public sealed record DatabaseReplicaProvisioningChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);