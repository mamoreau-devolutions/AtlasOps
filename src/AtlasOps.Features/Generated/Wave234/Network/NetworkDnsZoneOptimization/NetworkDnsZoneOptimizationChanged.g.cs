namespace AtlasOps.Features.Network.NetworkDnsZoneOptimization;

public sealed record NetworkDnsZoneOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);