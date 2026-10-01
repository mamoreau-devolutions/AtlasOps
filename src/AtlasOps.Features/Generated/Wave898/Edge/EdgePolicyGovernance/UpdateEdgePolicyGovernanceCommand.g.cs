namespace AtlasOps.Features.Edge.EdgePolicyGovernance;

public sealed record UpdateEdgePolicyGovernanceCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);