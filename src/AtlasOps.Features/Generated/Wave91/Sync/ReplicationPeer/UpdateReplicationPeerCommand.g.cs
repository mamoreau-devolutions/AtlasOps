namespace AtlasOps.Features.Sync.ReplicationPeer;

public sealed record UpdateReplicationPeerCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);