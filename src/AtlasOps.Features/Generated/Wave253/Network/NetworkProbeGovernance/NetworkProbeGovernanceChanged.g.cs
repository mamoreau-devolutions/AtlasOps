namespace AtlasOps.Features.Network.NetworkProbeGovernance;

public sealed record NetworkProbeGovernanceChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);