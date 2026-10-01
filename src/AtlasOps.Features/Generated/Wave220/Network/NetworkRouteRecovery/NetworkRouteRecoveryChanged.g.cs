namespace AtlasOps.Features.Network.NetworkRouteRecovery;

public sealed record NetworkRouteRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);