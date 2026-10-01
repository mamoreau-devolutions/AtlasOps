namespace AtlasOps.Features.Delivery.ReleaseRollbackGovernance;

public sealed record UpdateReleaseRollbackGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);