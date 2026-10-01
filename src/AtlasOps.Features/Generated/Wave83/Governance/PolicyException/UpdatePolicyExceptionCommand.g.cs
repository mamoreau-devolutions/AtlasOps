namespace AtlasOps.Features.Governance.PolicyException;

public sealed record UpdatePolicyExceptionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);