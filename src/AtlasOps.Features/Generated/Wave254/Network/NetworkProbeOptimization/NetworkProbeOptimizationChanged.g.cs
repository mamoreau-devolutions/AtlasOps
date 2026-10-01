namespace AtlasOps.Features.Network.NetworkProbeOptimization;

public sealed record NetworkProbeOptimizationChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);