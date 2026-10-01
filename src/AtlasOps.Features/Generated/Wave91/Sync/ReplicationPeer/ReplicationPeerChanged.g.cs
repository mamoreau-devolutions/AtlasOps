namespace AtlasOps.Features.Sync.ReplicationPeer;

public sealed record ReplicationPeerChanged(
    string EntityId,
    string PreviousState,
    string CurrentState,
    string Actor,
    DateTimeOffset OccurredAt);