namespace AtlasOps.Features.Edge.EdgePolicyRecovery;

public sealed record UpdateEdgePolicyRecoveryCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);