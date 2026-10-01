namespace AtlasOps.Features.Network.NetworkAddressOptimization;

public sealed record NetworkAddressOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);