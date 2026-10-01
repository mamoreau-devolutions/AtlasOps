namespace AtlasOps.Features.Delivery.ReleaseEnvironmentGovernance;

public sealed record UpdateReleaseEnvironmentGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);