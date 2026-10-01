namespace AtlasOps.Features.Cloud.CloudNetworkRecovery;

public sealed record CloudNetworkRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);