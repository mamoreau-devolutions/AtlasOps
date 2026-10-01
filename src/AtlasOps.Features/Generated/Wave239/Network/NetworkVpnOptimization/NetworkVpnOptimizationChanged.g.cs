namespace AtlasOps.Features.Network.NetworkVpnOptimization;

public sealed record NetworkVpnOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);