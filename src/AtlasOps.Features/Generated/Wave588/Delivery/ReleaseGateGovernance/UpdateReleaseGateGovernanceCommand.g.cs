namespace AtlasOps.Features.Delivery.ReleaseGateGovernance;

public sealed record UpdateReleaseGateGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);