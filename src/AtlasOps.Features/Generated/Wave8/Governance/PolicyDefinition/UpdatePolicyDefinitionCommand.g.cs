namespace AtlasOps.Features.Governance.PolicyDefinition;

public sealed record UpdatePolicyDefinitionCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);