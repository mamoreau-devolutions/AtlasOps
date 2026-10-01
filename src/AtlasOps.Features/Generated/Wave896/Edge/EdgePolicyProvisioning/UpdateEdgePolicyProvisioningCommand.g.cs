namespace AtlasOps.Features.Edge.EdgePolicyProvisioning;

public sealed record UpdateEdgePolicyProvisioningCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);