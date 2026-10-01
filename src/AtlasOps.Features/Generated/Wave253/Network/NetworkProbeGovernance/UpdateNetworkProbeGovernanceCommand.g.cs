namespace AtlasOps.Features.Network.NetworkProbeGovernance;

public sealed record UpdateNetworkProbeGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);