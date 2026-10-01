namespace AtlasOps.Features.Network.NetworkSegmentGovernance;

public sealed record UpdateNetworkSegmentGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);