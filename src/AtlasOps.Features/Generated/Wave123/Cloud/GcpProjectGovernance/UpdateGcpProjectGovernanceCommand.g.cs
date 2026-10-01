namespace AtlasOps.Features.Cloud.GcpProjectGovernance;

public sealed record UpdateGcpProjectGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);