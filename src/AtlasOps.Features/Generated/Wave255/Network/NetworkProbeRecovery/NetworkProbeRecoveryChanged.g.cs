namespace AtlasOps.Features.Network.NetworkProbeRecovery;

public sealed record NetworkProbeRecoveryChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);