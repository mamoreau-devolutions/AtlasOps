namespace AtlasOps.Features.Governance.OwnershipRule;

public sealed record UpdateOwnershipRuleCommand(
    string Id,
    string Name,
    string Owner,
    string TargetState,
    int Priority,
    bool IsEnabled);